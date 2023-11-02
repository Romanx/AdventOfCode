namespace DayTwentyTwo2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 22), "Monkey Game");

    public void PartOne(IInput input, IOutput output)
    {
        var (game, commands) = input.ParseGameFlat();

        foreach (var command in commands)
        {
            game.Apply(command);
        }

        var score = game.Score();
        output.WriteProperty("Password", score);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (game, commands) = input.ParseGameAsCube();

        foreach (var command in commands)
        {
            game.Apply(command);
        }

        var score = game.Score();
        output.WriteProperty("Password", score);
    }
}

internal abstract record Command();

internal record MoveCommand(int Number) : Command()
{
    public override string ToString()
        => $"Moving {Number}";
}

internal record TurnCommand(Direction Direction) : Command()
{
    public override string ToString()
        => $"Turning {Direction.DirectionType}";
}
