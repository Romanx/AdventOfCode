using Shared.Grid;

namespace DaySix2018;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 6), "Chronal Coordinates");

    public override void PartOne(IInput input, IOutput output)
    {
        var points = input.Parse();
        var area = Area2d.Create(points);
        var map = area.Items
            .Select(point =>
            {
                var closestByDistance = points
                    .MinBySet(other => PointHelpers.ManhattanDistance(point, other))
                    .ToArray();

                var closest = closestByDistance.Length > 1
                    ? null
                    : closestByDistance[0];

                return KeyValuePair.Create(point, closest);
            })
            .ToDictionary(k => k.Key, v => v.Value);

        var infinite = map
            .Where(kvp =>
                kvp.Key.X == area.XRange.Min ||
                kvp.Key.X == area.XRange.Max ||
                kvp.Key.Y == area.YRange.Min ||
                kvp.Key.Y == area.YRange.Max)
            .Where(kvp => kvp.Value is not null)
            .Select(kvp => kvp.Value!)
            .Distinct();

        var candidates = points.Except(infinite);

        var maxArea = map
            .Where(kvp => kvp.Value is not null && candidates.Contains(kvp.Value))
            .GroupBy(kvp => kvp.Value!)
            .Max(kvp => kvp.Count());

        output.WriteProperty("Max area", maxArea);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        const int LargestArea = 10000;

        var points = input.Parse();
        var area = Area2d.Create(points);

        var regions = new Dictionary<Point2d, int>();

        var regionSize = area.Items
            .Count(point =>
            {
                var sum = points.Sum(other => PointHelpers.ManhattanDistance(other, point));
                return sum < LargestArea;
            });

        output.WriteProperty("Region size", regionSize);
    }
}

internal static class ParseExtensions
{
    public static ImmutableHashSet<Point2d> Parse(this IInput input) => input.Lines
        .Transform(Point2d.Parse)
        .ToImmutableHashSet();
}
