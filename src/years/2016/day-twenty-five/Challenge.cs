using System.IO;
using Shared.Parser;
using Spectre.Console;

namespace DayTwentyFive2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 25), "Clock Signal");

        public override void PartOne(IInput input, IOutput output)
        {
            var commands = AssembunnyParser.BuildParser()
                .AddType<Out>()
                .ParseCommands(input.Lines)
                .ToImmutableArray();

            TextWriter display = new StringWriter();

            for (var i = 0; ; i++)
            {
                var registers = Program.EmptyRegisters;
                registers['a'] = i;

                var program = new Program(commands, registers, 0);

                var programOutput = program.RunWithOutput();

                display.WriteLine($"{i:D5}: {string.Join("", programOutput)}");

                if (IsValidSignal(programOutput))
                {
                    output.WriteProperty("Found repeating signal", i);
                    break;
                }
            }

            output.WriteBlock(() =>
            {
                return new Panel(display.ToString()!);
            });

            static bool IsValidSignal(IEnumerable<int> signal)
            {
                return signal
                    .Select((val, idx) =>
                    {
                        return idx % 2 == 0
                            ? val == 0
                            : val == 1;
                    }).All(x => x is true);
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }
    }

    [CommandRegex("out (.*)")]
    record Out(object[] Arguments) : Command("out", Arguments)
    {
        public int Value { get; private set; }

        public override void Apply(Program program)
        {
            Value = program.GetValueOrRegisterValue(Arguments[0]);
            program.Pointer++;
        }

        public static Out Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    static class ProgramExtensions
    {
        public static List<int> RunWithOutput(this Program program)
        {
            var output = new List<int>();
            var seen = new HashSet<(int, int, int, int, int)>();

            while (program.Pointer < program.Commands.Length)
            {
                var state = ValueTuple.Create(
                    program.Pointer,
                    program.Registers['a'],
                    program.Registers['b'],
                    program.Registers['c'],
                    program.Registers['d']);

                if (seen.Contains(state))
                {
                    break;
                }
                else
                {
                    seen.Add(state);
                }

                var instruction = program.Commands[program.Pointer];
                instruction.Apply(program);

                if (instruction is Out outInstruction)
                {
                    output.Add(outInstruction.Value);
                }
            }

            return output;
        }
    }
}
