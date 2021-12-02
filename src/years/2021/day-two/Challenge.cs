namespace DayTwo2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 2), "Dive!");

    public override void PartOne(IInput input, IOutput output)
    {
        var commands = input.Lines.ParseCommands();
        Submarine submarine = new(Position: 0, Depth: 0, Aim: 0);

        foreach (var command in commands)
        {
            submarine = command.Type switch
            {
                CommandType.Forward => submarine with { Position = submarine.Position + command.Units },
                CommandType.Up => submarine with { Depth = submarine.Depth - command.Units },
                CommandType.Down => submarine with { Depth = submarine.Depth + command.Units },
                _ => throw new NotImplementedException(),
            };
        }

        output.WriteProperty("Depth", submarine.Depth);
        output.WriteProperty("Position", submarine.Position);

        output.WriteProperty("Result", submarine.Depth * submarine.Position);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var commands = input.Lines.ParseCommands();
        Submarine submarine = new(Position: 0, Depth: 0, Aim: 0);

        foreach (var command in commands)
        {
            submarine = command.Type switch
            {
                CommandType.Forward => submarine with
                {
                    Position = submarine.Position + command.Units,
                    Depth = submarine.Depth + (submarine.Aim * command.Units)
                },
                CommandType.Up => submarine with { Aim = submarine.Aim + command.Units },
                CommandType.Down => submarine with { Aim = submarine.Aim - command.Units },
                _ => throw new NotImplementedException(),
            };
        }

        output.WriteProperty("Depth", submarine.Depth);
        output.WriteProperty("Position", submarine.Position);

        output.WriteProperty("Result", Math.Abs(submarine.Depth) * submarine.Position);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Command> ParseCommands(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<Command>();
        foreach (var line in lines.AsMemory())
        {
            var span = line.Span;
            var split = span.IndexOf(' ');

            var type = Enum.Parse<CommandType>(span[..split], ignoreCase: true);
            var units = int.Parse(span[split..]);

            builder.Add(new Command(type, units));
        }

        return builder.ToImmutable();
    }
}

readonly record struct Submarine(int Position, int Depth, int Aim);

readonly record struct Command(CommandType Type, int Units);

enum CommandType
{
    Forward,
    Up,
    Down,
}
