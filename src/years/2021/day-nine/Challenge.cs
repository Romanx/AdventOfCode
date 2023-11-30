namespace DayNine2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 9), "Smoke Basin");

    public void PartOne(IInput input, IOutput output)
    {
        var map = input.As2DPoints()
            .ToImmutableDictionary(k => k.Point, v => v.Character - '0');

        var lowPoints = new HashSet<Point2d>();

        foreach (var (point, height) in map)
        {
            var neighbours = point.GetNeighbours(AdjacencyType.Cardinal);

            var lowest = neighbours // Go through all possible neighbours
                .Select(n => map.TryGetValue(n, out var neighbourHeight) ? neighbourHeight : (int?)null) // Get their height or null for missing.
                .Where(n => n is not null)
                .All(neighbourHeight => height < neighbourHeight);

            if (lowest)
            {
                lowPoints.Add(point);
            }
        }

        var score = lowPoints
            .Sum(point => 1 + map[point]);

        output.WriteProperty("Risk Level", score);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var map = input.As2DPoints()
            .ToImmutableDictionary(k => k.Point, v => v.Character - '0');

        var validPoints = map
            .Where(kvp => kvp.Value != 9)
            .Select(kvp => kvp.Key)
            .ToHashSet();

        var basins = new List<HashSet<Point2d>>();

        while (validPoints.Count > 0)
        {
            var current = validPoints.First();
            var basin = FindBasinContainingPoint(current, map);

            basins.Add(basin);

            validPoints.ExceptWith(basin);
        }

        var sizes = basins
            .OrderByDescending(basin => basin.Count)
            .Take(3)
            .Aggregate(1, (seed, set) => seed * set.Count);

        output.WriteProperty("Largest Basins Multiplied", sizes);

        static HashSet<Point2d> FindBasinContainingPoint(Point2d source, IReadOnlyDictionary<Point2d, int> map)
        {
            var currentFrontier = new List<Point2d>();
            var nextFrontier = new List<Point2d>();
            currentFrontier.Add(source);
            var visited = new HashSet<Point2d>
            {
                source
            };

            while (currentFrontier.Count > 0)
            {
                foreach (var current in currentFrontier)
                {
                    foreach (var next in current.GetNeighbours(AdjacencyType.Cardinal))
                    {
                        if (map.TryGetValue(next, out var nextHeight) && nextHeight != 9)
                        {
                            if (visited.Add(next) is true)
                            {
                                nextFrontier.Add(next);
                            }
                        }
                    }
                }

                (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
                nextFrontier.Clear();
            }

            return visited;
        }
    }
}
