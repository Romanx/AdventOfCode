using MoreLinq;
using Shared.Grid;

namespace DayThree2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 3), "No Matter How You Slice It");

    public void PartOne(IInput input, IOutput output)
    {
        var claims = input.Parse();

        var totalClaimedByMultiple = claims
            .SelectMany(claim => claim.Area.Items)
            .CountBy(point => point)
            .Count(kvp => kvp.Value > 1);

        output.WriteProperty("Number of overlapping points", totalClaimedByMultiple);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var claims = input.Parse();

        var special = claims
            .Single(claim =>
            {
                foreach (var other in claims.Where(c => c.Id != claim.Id))
                {
                    if (claim.Area.Intersects(other.Area))
                    {
                        return false;
                    }
                }

                return true;
            });

        output.WriteProperty("Remaining claim", special.Id);
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"#(\d+) @ (\d+,\d+): (\d+)x(\d+)");

    public static ImmutableArray<Claim> Parse(this IInput input)
    {
        var result = ImmutableArray.CreateBuilder<Claim>();
        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);

            var id = int.Parse(match.Groups[1].Value);
            var left = Point2d.Parse(match.Groups[2].Value);

            var x = int.Parse(match.Groups[3].Value);
            var y = int.Parse(match.Groups[4].Value);
            var bottomRight = left + new Point2d(x - 1, y - 1);

            result.Add(new Claim(id, Area2d.Create(left, bottomRight)));
        }

        return result.ToImmutable();
    }
}

record Claim(int Id, Area2d Area);
