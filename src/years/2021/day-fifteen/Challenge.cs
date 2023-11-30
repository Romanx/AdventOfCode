using Shared.Graph;
using Shared.Grid;

namespace DayFifteen2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 15), "Chiton");

    public void PartOne(IInput input, IOutput output)
    {
        var map = input.ParseMap();
        var area2d = Area2d.Create(map.Keys);

        var graph = new Graph(map);
        var path = graph.AStarSearch(
            area2d.TopLeft,
            area2d.BottomRight,
            static (a, b) => PointHelpers.ManhattanDistance(a, b));

        var totalCost = path.Skip(1)
            .Sum(point => map[point]);

        output.WriteProperty("Total Cost", totalCost);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var map = input.ParseFullMap();
        var area2d = Area2d.Create(map.Keys);

        var graph = new Graph(map);
        var path = graph.AStarSearch(
            area2d.TopLeft,
            area2d.BottomRight,
            static (a, b) => PointHelpers.ManhattanDistance(a, b));

        var totalCost = path.Skip(1)
            .Sum(point => map[point]);

        output.WriteProperty("Total Cost", totalCost);
    }
}

internal static class ParseExtensions
{
    public static ImmutableDictionary<Point2d, int> ParseMap(this IInput input)
        => input.As2DPoints()
            .ToImmutableDictionary(k => k.Point, v => v.Character - '0');

    public static ImmutableDictionary<Point2d, int> ParseFullMap(this IInput input)
    {
        var map = input.ParseMap();
        var area2d = Area2d.Create(map.Keys);
        var fullMap = ImmutableDictionary.CreateBuilder<Point2d, int>();

        for (var fy = 0; fy < 5; fy++)
        {
            var yOffset = (area2d.BottomLeft.Y + 1) * fy;

            for (var fx = 0; fx < 5; fx++)
            {
                var xOffset = (area2d.TopRight.X + 1) * fx;

                for (var y = 0; y < area2d.Height; y++)
                {
                    for (var x = 0; x < area2d.Width; x++)
                    {
                        var point = new Point2d(x + xOffset, y + yOffset);
                        var risk = map[(x, y)];
                        fullMap[point] = CalculateRisk(risk, fx, fy);
                    }
                }
            }
        }

        return fullMap.ToImmutable();

        static int CalculateRisk(int risk, int xOffset, int yOffset)
        {
            // To clamp between range 1 - 9 we subtract one before the modulo and then add one after.
            return ((risk + xOffset + yOffset - 1) % 9) + 1;
        }
    }
}

record class Graph(ImmutableDictionary<Point2d, int> Map) : IWeightedGraph<Point2d>
{
    public int Cost(Point2d nodeA, Point2d nodeB)
        => Map[nodeA] + Map[nodeB];

    public IEnumerable<Point2d> Neighbours(Point2d node)
    {
        if (Map.ContainsKey(node) is false)
        {
            return Enumerable.Empty<Point2d>();
        }

        return node.GetNeighbours(AdjacencyType.Cardinal)
            .Where(point => Map.ContainsKey(point));
    }
}
