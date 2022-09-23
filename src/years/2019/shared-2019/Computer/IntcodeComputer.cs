using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Helpers.Instructions;

namespace Helpers
{
    public class IntcodeComputer
    {
        private static readonly Dictionary<OpCodes, Instruction> validInstructions = BuildInstructions();
        private readonly ImmutableArray<long> _program;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly List<Task> tasks = new();
        const int HaltCode = 99;

        private long[] _memory;
        private int _index = 0;
        private int _relativeBase = 0;

        public Channel<long> Input { get; }

        public Channel<long> Output { get; }

        public IntcodeComputer(ImmutableArray<long> memory)
        {
            _program = memory;
            _memory = SetProgram(_program);
            var opts = new UnboundedChannelOptions { AllowSynchronousContinuations = false };
            Input = Channel.CreateUnbounded<long>(opts);
            Output = Channel.CreateUnbounded<long>(opts);
            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task<IntcodeResult> Run(CancellationToken token = default)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationTokenSource.Token,
                token);

            try
            {
                while (_memory[_index] != HaltCode)
                {
                    var op = _memory[_index];
                    var (opCode, parameterModes) = ParseOperation(op);

                    if (validInstructions.TryGetValue(opCode, out var instruction))
                    {
                        var @params = GetParameters(instruction.Parameters, parameterModes);
                        await instruction.RunInstruction(@params, this, cts.Token);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid op code: {opCode}");
                    }
                }

                Input.Writer.Complete();
                Output.Writer.Complete();
                cancellationTokenSource.Cancel();

                await Task.WhenAll(tasks);
                return IntcodeResult.HALT_TERMINATE;
            }
            catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
            {
                Input.Writer.Complete();
                Output.Writer.Complete();
                cancellationTokenSource.Cancel();

                return IntcodeResult.HALT_TERMINATE;
            }
        }

        public async Task<IList<long>> RunAndGetOutput()
        {
            await Run();
            var result = await Output.Reader.ReadAllAsync().ToListAsync();
            return result;
        }

        public void SetIndex(int index) => _index = index;

        public long ReadMemory(int position) => _memory[position];

        public void WriteToMemory(int position, long value)
        {
            if (position >= _memory.Length)
            {
                IncreaseMemory();
            }

            _memory[position] = value;
        }

        public void AdjustIndexBy(int value) => _index += value;

        public void AdjustRelativeBaseBy(int value) => _relativeBase += value;

        private static (OpCodes opCode, int[] parameterModes) ParseOperation(long op)
        {
            var operation = op.ToString().ToArray();
            if (operation.Length == 1)
            {
                return ((OpCodes)op, Array.Empty<int>());
            }

            var opCode = int.Parse(operation.AsSpan()[^2..]);

            var parameters = operation[..^2].Select(c => c - '0').Reverse().ToArray();

            return ((OpCodes)opCode, parameters);
        }

        private ReadOnlyMemory<long> GetParameters(
            ParameterType[] parameterTypes,
            in ReadOnlySpan<int> parameterModes)
        {
            var paramStart = _index + 1;
            var readParameters = _memory[paramStart..(paramStart + parameterTypes.Length)];

            var modes = new int[parameterTypes.Length];
            modes.AsSpan().Fill(0);
            parameterModes.CopyTo(modes);

            var results = new long[parameterTypes.Length];

            for (var i = 0; i < readParameters.Length; i++)
            {
                var type = parameterTypes[i];
                var param = readParameters[i];
                var mode = (ParameterMode)modes[i];

                switch (mode)
                {
                    case ParameterMode.PositionMode when type == ParameterType.Write:
                        results[i] = param;
                        break;
                    case ParameterMode.PositionMode:
                        if (param > _memory.Length)
                            IncreaseMemory();

                        results[i] = _memory[param];
                        break;
                    case ParameterMode.ImmediateMode:
                        if (type == ParameterType.Write)
                            throw new InvalidOperationException("Cannot write with immedate mode");

                        results[i] = param;
                        break;
                    case ParameterMode.RelativeMode when type == ParameterType.Write:
                        results[i] = _relativeBase + param;
                        break;

                    case ParameterMode.RelativeMode:
                        if (_relativeBase + param > _memory.Length)
                            IncreaseMemory();
                        results[i] = _memory[_relativeBase + param];
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Memory Mode!");
                }
            }

            return results;
        }

        private static long[] SetProgram(ImmutableArray<long> program)
        {
            var scratch = new long[program.Length];
            scratch.AsSpan().Fill(0);
            program.CopyTo(scratch);
            return scratch;
        }

        private void IncreaseMemory()
        {
            var newMemory = new long[(int)Math.Pow(_memory.Length, 2)];
            newMemory.AsSpan().Fill(0);
            _memory.CopyTo(newMemory.AsSpan());
            _memory = newMemory;
        }

        private static Dictionary<OpCodes, Instruction> BuildInstructions()
        {
            return typeof(Instruction)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(Instruction).IsAssignableFrom(t))
                .Select(t => (Instruction)Activator.CreateInstance(t)!)
                .ToDictionary(k => k.OpCode, v => v);
        }

        public void AddInputReader(ChannelReader<long> reader)
        {
            var task = Task.Run(async () =>
            {
                await foreach (var i in reader.ReadAllAsync())
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    await Input.Writer.WriteAsync(i);
                }
            });

            tasks.Add(task);
        }
    }

    internal enum ParameterMode
    {
        PositionMode = 0,
        ImmediateMode = 1,
        RelativeMode = 2
    }
}
