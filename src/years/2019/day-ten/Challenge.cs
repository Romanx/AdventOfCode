using static Shared.PointHelpers;

namespace DayTen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 10), "Monitoring Station");

    public void PartOne(IInput input, IOutput output)
    {
        var map = input.Parse();

        var (asteroid, visible) = map
            .Select(p => (Point: p, Visible: CountVisibleAsteroids(map, p)))
            .MaxBy(i => i.Visible);

        output.WriteProperty("Best asteroid at", asteroid);
        output.WriteProperty("Number of visible other asteroids", visible);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var map = input.Parse();

        var (station, _) = map
            .Select(p => (Point: p, Visible: CountVisibleAsteroids(map, p)))
            .MaxBy(i => i.Visible);

        var destroyed = DestroyedOrderList(map, station);

        var targetAsteriod = destroyed[199];

        output.WriteProperty("Asteroid 200 was", targetAsteriod);
        output.WriteProperty("Result", (targetAsteriod.X * 100) + targetAsteriod.Y);

        static ImmutableArray<Point2d> DestroyedOrderList(ImmutableHashSet<Point2d> map, Point2d station)
        {
            var sorted = map.Where(p => p != station)
                .GroupBy(o => AngleInDegrees(station, o))
                .Select(g => (Angle: g.Key, Queue: new Queue<Point2d>(g.OrderBy(x => ManhattanDistance(x, station)))))
                .OrderBy(x => x.Angle)
                .ToImmutableArray();

            var banged = ImmutableArray.CreateBuilder<Point2d>(map.Count - 1);

            while (sorted.Length > 0)
            {
                foreach (var (_, asteroids) in sorted)
                {
                    var asteroid = asteroids.Dequeue();

                    banged.Add(asteroid);
                }

                sorted = sorted.RemoveAll(static i => i.Queue.Count == 0);
            }

            return banged.MoveToImmutable();
        }
    }

    private static int CountVisibleAsteroids(ImmutableHashSet<Point2d> map, Point2d point)
    {
        return map
            .Where(p => p != point)
            .GroupBy(o => AngleInRadians(point, o))
            .Count();
    }
}

internal static class ParseExtensions
{
    public static ImmutableHashSet<Point2d> Parse(this IInput input)
    {
        return input.As2DPoints()
            .Where(item => item.Character is '#')
            .Select(item => item.Point)
            .ToImmutableHashSet();
    }
}
