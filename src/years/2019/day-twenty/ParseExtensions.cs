using CommunityToolkit.HighPerformance;
using Shared.Grid;

namespace DayTwenty2019;

internal static class ParseExtensions
{
    public static Maze Parse(this IInput input, bool recursive)
    {
        var arr = input.Lines
            .As2DArray()
            .AsSpan2D();

        var open = ImmutableHashSet.CreateBuilder<Point2d>();
        var portalPoints = new HashSet<Point2d>();

        for (var y = 0; y < arr.Height; y++)
        {
            for (var x = 0; x < arr.Width; x++)
            {
                Point2d point = (x, y);

                var @char = arr[y, x];
                if (@char is '.')
                {
                    open.Add(point);
                }
                else if (char.IsLetter(@char) && char.IsUpper(@char))
                {
                    portalPoints.Add(point);
                }
            }
        }

        var portals = BuildPortals(arr, portalPoints, open);
        var entrancePortal = portals.Find(p => p.Identifier is "AA");
        var exitPortal = portals.Find(p => p.Identifier is "ZZ");
        portals.Remove(entrancePortal);
        portals.Remove(exitPortal);

        var entrance = entrancePortal.Position.GetNeighbours(open, AdjacencyType.Cardinal)
            .First();

        var exit = exitPortal.Position.GetNeighbours(open, AdjacencyType.Cardinal)
            .First();

        var portalPairs = MergePortals(portals, open);

        return new Maze(
            entrance.Z(0),
            exit.Z(0),
            open.ToImmutable(),
            portalPairs,
            recursive);
    }

    private static ImmutableArray<PortalPair> MergePortals(
        List<Portal> points,
        ISet<Point2d> openSpace)
    {
        var portals = ImmutableArray.CreateBuilder<PortalPair>(points.Count);

        while (points.Count > 0)
        {
            var portalA = points[0];
            var portalB = points.Find(pp => pp.Identifier == portalA.Identifier && pp.Position != portalA.Position);

            points.Remove(portalA);
            points.Remove(portalB);

            portals.Add(new PortalPair(
                portalA.Identifier,
                portalA.Position,
                portalB.Position.GetNeighbours(openSpace, AdjacencyType.Cardinal).First(),
                portalA.IsInner));

            portals.Add(new PortalPair(
                portalB.Identifier,
                portalB.Position,
                portalA.Position.GetNeighbours(openSpace, AdjacencyType.Cardinal).First(),
                portalB.IsInner));
        }

        return portals.MoveToImmutable();
    }

    private static List<Portal> BuildPortals(
        Span2D<char> arr,
        HashSet<Point2d> portalPoints,
        ISet<Point2d> openPoints)
    {
        var portals = new List<Portal>(portalPoints.Count);
        var area = Area2d.Create(openPoints);

        while (portalPoints.Count > 0)
        {
            var first = portalPoints.First();

            foreach (var second in first.GetNeighbours(AdjacencyType.Cardinal))
            {
                if (portalPoints.Contains(second))
                {
                    portalPoints.Remove(first);
                    portalPoints.Remove(second);
                    var firstId = arr[first.Y, first.X];
                    var secondId = arr[second.Y, second.X];

                    // If the area contains the portal then it's inner
                    var isInner = area.Contains(first);

                    var identifier = first switch
                    {
                        _ when first.X > second.X => $"{secondId}{firstId}",
                        _ when first.X < second.X => $"{firstId}{secondId}",
                        _ when first.Y > second.Y => $"{secondId}{firstId}",
                        _ when second.Y > first.Y => $"{firstId}{secondId}",
                        _ => throw new InvalidOperationException("Didn't handle merge correctly")
                    };

                    var firstAdjacent = first.GetNeighbours(openPoints, AdjacencyType.Cardinal);

                    if (firstAdjacent.Count is 1)
                    {
                        portals.Add(new Portal(identifier, first, isInner));
                    }
                    else
                    {
                        portals.Add(new Portal(identifier, second, isInner));
                    }
                    break;
                }
            }
        }

        return portals;
    }

    private readonly record struct Portal(string Identifier, Point2d Position, bool IsInner);
}
