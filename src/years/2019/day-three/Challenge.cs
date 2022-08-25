using Microsoft.Toolkit.HighPerformance;

namespace DayThree2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 3), "Crossed Wires");

    public void PartOne(IInput input, IOutput output)
    {
        var (first, second) = input.ParseWires();
        var collisions = first.Intersections(second);
        var closest = collisions
            .MinBy(x => PointHelpers.ManhattanDistance(Point2d.Origin, x))!;

        output.WriteProperty("Closest to Origin", closest);
        output.WriteProperty("Distance", PointHelpers.ManhattanDistance(Point2d.Origin, closest));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (first, second) = input.ParseWires();
        var collisions = first.Intersections(second);

        var firstFullPath = first.Path.SelectMany(segment => segment.Points)
            .ToArray();
        var secondFullPath = second.Path.SelectMany(segment => segment.Points)
            .ToArray();

        long shortest = collisions
            .Select(collision =>
            {
                var firstIndex = firstFullPath.AsSpan().IndexOf(collision) + 1;
                var secondIndex = secondFullPath.AsSpan().IndexOf(collision) + 1;

                return firstIndex + secondIndex;
            })
            .Min();

        output.WriteProperty("Total Steps", shortest);
    }

    public ImmutableArray<int> GetIntersectionSteps(ImmutableArray<Point2d>[] lines)
    {
        var results = ImmutableArray.CreateBuilder<int>();
        for (var i = 0; i < lines.Length - 1; i++)
        {
            var line = lines[i];
            var rest = lines[(i + 1)..^0];

            foreach (var other in rest)
            {
                var intersections = line.Intersect(other);
                foreach (var intersection in intersections)
                {
                    var idx = line.IndexOf(intersection) + 1;
                    var idx2 = other.IndexOf(intersection) + 1;
                    results.Add(idx + idx2);
                }
            }
        }

        return results.ToImmutable();
    }
}
