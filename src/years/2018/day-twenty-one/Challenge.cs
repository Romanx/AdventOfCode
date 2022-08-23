using static Shared.DeviceParser;
using static Shared.InstructionHelper;

namespace DayTwentyOne2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 21), "Chronal Conversion");

        public void PartOne(IInput input, IOutput output)
        {
            var (instructionPointerBinding, commands) = Parse(input);
            var targetCommand = FindCommandUsingRegisterZero(commands);
            var number = Execute(instructionPointerBinding, commands, targetCommand).First();

            output.WriteProperty("Value of register", number);
        }
        public void PartTwo(IInput input, IOutput output)
        {
            var (instructionPointerBinding, commands) = Parse(input);
            var targetCommand = FindCommandUsingRegisterZero(commands);
            var values = Execute(instructionPointerBinding, commands, targetCommand);

            var number = values.Last();
            output.WriteProperty("Value of register", number);
        }

        private static IEnumerable<int> Execute(int instructionPointerBinding, ImmutableArray<Command> commands, Command targetCommand)
        {
            const int magicRegister = 1;
            var registers = new int[6];
            registers.AsSpan().Fill(0);
            var seen = new HashSet<int>();

            var ip = registers[instructionPointerBinding];
            while ((0..commands.Length).Contains(ip))
            {
                var command = commands[ip];
                var instruction = InstructionMap[command.Instruction];
                registers[instructionPointerBinding] = ip;
                instruction.Action(registers, command.A, command.B, command.C);
                ip = registers[instructionPointerBinding] + 1;
                if (commands[ip] == targetCommand)
                {
                    var value = registers[magicRegister];
                    if (seen.Contains(value))
                    {
                        yield return seen.Last();
                        break;
                    }
                    seen.Add(value);
                    yield return value;
                }
            }
        }

        private static Command FindCommandUsingRegisterZero(ImmutableArray<Command> commands)
        {
            foreach (var command in commands)
            {
                switch (command.Instruction)
                {
                    case "addr" when command.A == 0 || command.B == 0:
                    case "mulr" when command.A == 0 || command.B == 0:
                    case "banr" when command.A == 0 || command.B == 0:
                    case "borr" when command.A == 0 || command.B == 0:
                    case "setr" when command.A == 0:
                    case "gtir" when command.B == 0:
                    case "gtrr" when command.A == 0 || command.B == 0:
                    case "eqir" when command.B == 0:
                    case "eqrr" when command.A == 0 || command.B == 0:
                        return command;
                }
            }

            throw new InvalidOperationException();
        }

    }

    internal static class ParseExtensions
    {
    }
}
