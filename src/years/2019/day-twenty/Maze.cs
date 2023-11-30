using Shared.Graph;
using Spectre.Console;

namespace DayTwenty2019;

public class Maze : IWeightedGraph<Point3d>
{
    private readonly bool recursive;
    private readonly ImmutableHashSet<Point2d> openSpace;
    private readonly ImmutableDictionary<Point2d, PortalPair> portals;

    public Maze(
        Point3d entrance,
        Point3d exit,
        ImmutableHashSet<Point2d> openSpace,
        ImmutableArray<PortalPair> portals,
        bool recursive)
    {
        Entrance = entrance;
        Exit = exit;
        this.openSpace = openSpace;
        this.recursive = recursive;
        this.portals = portals.ToImmutableDictionary(k => k.Entrance);
    }

    public Point3d Entrance { get; }

    public Point3d Exit { get; }

    public IEnumerable<Point3d> Neighbours(Point3d node) => recursive
        ? RecursiveNeighbours(node)
        : NormalNeighbours(node);

    private IEnumerable<Point3d> NormalNeighbours(Point3d node)
    {
        foreach (var neighbour in Directions.CardinalDirections.Select(dir => node + dir))
        {
            if (portals.TryGetValue(neighbour, out var portal))
            {
                yield return portal.Exit.Z(node.Z);
            }
            else if (openSpace.Contains(neighbour))
            {
                yield return neighbour;
            }
        }
    }

    private IEnumerable<Point3d> RecursiveNeighbours(Point3d node)
    {
        foreach (var neighbour in Directions.CardinalDirections.Select(dir => node + dir))
        {
            var hasPortal = portals.TryGetValue(neighbour, out var portal);

            if (hasPortal && portal.IsInner)
            {
                yield return portal.Exit.Z(node.Z + 1);
            }
            else if (hasPortal && node.Z is not 0)
            {
                yield return portal.Exit.Z(node.Z - 1);
            }
            else if (openSpace.Contains(neighbour))
            {
                yield return neighbour;
            }
        }
    }

    public int Cost(Point3d nodeA, Point3d nodeB) => 1;
}

public readonly record struct PortalPair(
    string Identifier,
    Point2d Entrance,
    Point2d Exit,
    bool IsInner);
