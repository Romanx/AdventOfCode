using Shared.Grid;

namespace DayThree2019;

public readonly record struct Wire(ImmutableArray<LineSegment> Path)
{
    public ImmutableHashSet<Point2d> Intersections(Wire other)
    {
        var collisions = ImmutableHashSet.CreateBuilder<Point2d>();

        foreach (var segment in Path)
        {
            foreach (var otherSegment in other.Path)
            {
                if (segment.Intersects(otherSegment, out var collision))
                {
                    collisions.Add(collision.Value);
                }
            }
        }

        return collisions.ToImmutable();
    }
}
