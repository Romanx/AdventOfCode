namespace DayEight2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 8), "Memory Maneuver");

    public void PartOne(IInput input, IOutput output)
    {
        var node = input.Parse();

        output.WriteProperty("Total metadata sum", SumMetadata(node));

        static int SumMetadata(Node node)
        {
            var total = node.Metadata.Sum();
            foreach (var child in node.Children)
            {
                total += SumMetadata(child);
            }

            return total;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var node = input.Parse();

        output.WriteProperty("Root node value", node.Value);
    }
}
