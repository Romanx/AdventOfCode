namespace DayEighteen2021;
using static MoreLinq.Extensions.SubsetsExtension;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 18), "Snailfish");

    public void PartOne(IInput input, IOutput output)
    {
        var result = input.Lines
            .AsMemory()
            .Select(line => SnailfishNumber.Parse(line.Span))
            .Aggregate((a, b) => a + b);

        output.WriteProperty("Result", result);
        output.WriteProperty("Magnitude", result.Magnitude());
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var memory = input.Lines.AsMemory();
        var candidates = memory
            .Subsets(2)
            .SelectMany(candidates =>
            {
                return new[]
                {
                    (First: SnailfishNumber.Parse(candidates[0].Span), Second: SnailfishNumber.Parse(candidates[1].Span)),
                    (First: SnailfishNumber.Parse(candidates[1].Span), Second: SnailfishNumber.Parse(candidates[0].Span))
                };
            });

        var magnitude = candidates
            .Max(candidate => CalculateMagnitude(candidate.First, candidate.Second));

        output.WriteProperty("Max Magnitude", magnitude);

        static int CalculateMagnitude(SnailfishNumber first, SnailfishNumber second)
        {
            return (first + second).Magnitude();
        }
    }
}
