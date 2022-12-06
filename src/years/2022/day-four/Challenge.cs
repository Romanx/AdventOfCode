namespace DayFour2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 4), "Camp Cleanup");

    public void PartOne(IInput input, IOutput output)
    {
        var assignments = input.Lines
            .As<Assignment>()
            .ToImmutableArray();

        var numberOfWholeOverlaps = assignments
            .Count(a => a.HasWholeOverlap);

        output.WriteProperty("Number of entirely overlapping assignments", numberOfWholeOverlaps);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var assignments = input.Lines
            .As<Assignment>()
            .ToImmutableArray();

        var numberOfPartialOverlaps = assignments
            .Count(a => a.HasPartialOverlap);

        output.WriteProperty("Number of partially overlapping assignments", numberOfPartialOverlaps);
    }
}
