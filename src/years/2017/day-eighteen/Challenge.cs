using System.Threading.Channels;
using System.Threading.Tasks;
using Shared.Parser;

namespace DayEighteen2017
{
    public class Challenge : ChallengeAsync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 18), "Duet");

        public override async Task PartOne(IInput input, IOutput output)
        {
            var instructions = input.Lines.ParseInstructions();
            var channel = Channel.CreateUnbounded<long>();

            var program = new Program(0, channel.Writer, channel.Reader);

            var recoveredFrequency = await RunUntilFirstOutput(program, instructions);

            output.WriteProperty("Recovered Frequency", recoveredFrequency);

            static async ValueTask<long?> RunUntilFirstOutput(Program program, ImmutableArray<Instruction> instructions)
            {
                while (program.Pointer >= 0 && program.Pointer < instructions.Length)
                {
                    var instruction = instructions[program.Pointer];

                    if (instruction is RecoverSound rs)
                    {
                        var value = await rs.ApplyPart1(program);
                        if (value is not null)
                        {
                            return value.Value;
                        }
                    }
                    else
                    {
                        await instruction.Apply(program);
                    }

                    program.Pointer++;
                }

                return null;
            }
        }

        public override async Task PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Lines.ParseInstructions();
            var program0IO = Channel.CreateUnbounded<long>();
            var program1IO = Channel.CreateUnbounded<long>();

            var programZero = new Program(0, program0IO.Writer, program1IO.Reader);
            programZero.Registers['p'] = 0;

            var programOne = new Program(1, program1IO.Writer, program0IO.Reader);
            programOne.Registers['p'] = 1;

            await Task.WhenAll(
                RunUntilEnd(programZero, instructions),
                RunUntilEnd(programOne, instructions));

            output.WriteProperty("Number of sends from Program One", programOne.Sends);

            static async Task RunUntilEnd(Program program, ImmutableArray<Instruction> instructions)
            {
                try
                {
                    while (program.Pointer >= 0 && program.Pointer < instructions.Length)
                    {
                        var instruction = instructions[program.Pointer];
                        await instruction.Apply(program);

                        program.Pointer++;
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Instruction> ParseInstructions(this IInputLines lines)
        {
            var parser = new CommandParser<Instruction>()
                .AddDerivedTypes<Instruction>();

            return parser.ParseCommands(lines)
                .ToImmutableArray();
        }
    }
}
