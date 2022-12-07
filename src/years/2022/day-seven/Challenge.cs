namespace DaySeven2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 7), "No Space Left On Device");

    public void PartOne(IInput input, IOutput output)
    {
        var root = OutputParser.ParseFromOutput(input.Lines.AsArray());

        var size = root.Flatten()
            .Where(dir => dir.Size <= 100_000)
            .Sum(dir => dir.Size);

        output.WriteProperty("Total size", size);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const long TotalDiskSize = 7_0000_000;
        const long RequiredSpace = 3_0000_000;

        var root = OutputParser.ParseFromOutput(input.Lines.AsArray());

        var currentSpace = TotalDiskSize - root.Size;
        var neededSpace = RequiredSpace - currentSpace;

        var toDelete = root.Flatten()
            .Where(dir => dir.Size >= neededSpace)
            .OrderBy(dir => dir.Size)
            .First();

        output.WriteProperty("Directory to delete", toDelete.Name);
        output.WriteProperty("Size of Directory to Delete", toDelete.Size);
    }
}
