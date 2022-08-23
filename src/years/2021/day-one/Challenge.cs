namespace DayOne2021;
using static MoreLinq.Extensions.WindowExtension;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 1), "Sonar Sweep");

    public void PartOne(IInput input, IOutput output)
    {
        var distances = input.Lines.Ints();

        var changes = distances
            .Window(2)
            .Select(x => CalculateChange(x[1], x[0]));

        output.WriteProperty("Number of Increases", changes.Count(x => x is ChangeType.Increase));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var distances = input.Lines.Ints();

        var changes = distances
            .Window(3)
            .Select(x => x.Sum())
            .Window(2)
            .Select(x => CalculateChange(x[1], x[0]));

        output.WriteProperty("Number of Increases", changes.Count(x => x is ChangeType.Increase));
    }

    private static ChangeType CalculateChange(int current, int previous)
    {
        if (current == previous)
        {
            return ChangeType.NoChange;
        }
        else if (current > previous)
        {
            return ChangeType.Increase;
        }
        else
        {
            return ChangeType.Decrease;
        }
    }
}

enum ChangeType
{
    NoChange,
    Increase,
    Decrease,
}
