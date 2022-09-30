using System.Text;
using System.Threading.Tasks;
using Helpers;
using Shared.Extensions;

namespace Shared2019.Computer
{
    public sealed class AsciiComputer
    {
        private static readonly int newline = '\n';
        private readonly IntcodeComputer _intcodeComputer;

        public AsciiComputer(ImmutableArray<long> memory)
        {
            _intcodeComputer = new IntcodeComputer(memory);
            _ = _intcodeComputer.Run();
        }

        public async Task<string> EnqueueCommandAndRun(string command)
        {
            await EnqueueCommand(command);
            var commandResult = await WaitAndReadResult();
            return commandResult;

            async Task EnqueueCommand(string command)
            {
                foreach (var c in command.ToCharArray())
                {
                    await _intcodeComputer.Input.Writer.WriteAsync(c);
                }

                await _intcodeComputer.Input.Writer.WriteAsync(newline);
            }
        }

        public async Task<string> WaitAndReadResult()
        {
            var result = new StringBuilder();
            var reader = _intcodeComputer.Output.Reader;
            await foreach (var read in reader.ReadAllAsync())
            {
                var @char = (char)read;

                result.Append(@char);
                if (result.EndsWith("Command?\n", StringComparison.OrdinalIgnoreCase))
                {
                    return result.ToString();
                }
            }

            return result.ToString();
        }
    }
}
