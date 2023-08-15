using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared;

public readonly struct AdjacentPoints3d : IEnumerable<Point3d>
{
    private readonly Point3d[] adjacentPoints;

    public AdjacentPoints3d(Point3d centre)
        : this (centre, ImmutableHashSet<Point3d>.Empty)
    {
    }

    public AdjacentPoints3d(Point3d centre, IReadOnlySet<Point3d> points)
    {
        Centre = centre;

        var list = new List<Point3d>(Offsets.Length);
        foreach (var offset in Offsets)
        {
            var adjacent = centre + offset;

            var shouldAdd = points.Count is 0 || points.Contains(adjacent);

            if (shouldAdd)
            {
                list.Add(adjacent);
            }
        }

        adjacentPoints = list.ToArray();
    }

    public Point3d Centre { get; }

    public int Count() => adjacentPoints.Length;

    public int Count(IReadOnlySet<Point3d> points)
    {
        var count = 0;
        foreach (var point in adjacentPoints)
        {
            if (points.Contains(point))
            {
                count++;
            }
        }

        return count;
    }

    public IEnumerator<Point3d> GetEnumerator()
    {
        foreach (var point in adjacentPoints)
        {
            yield return point;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private static readonly Point3d[] Offsets = new Point3d[]
    {
        (1, 0, 0),
        (-1, 0, 0),
        (0, 1, 0),
        (0, -1, 0),
        (0, 0, 1),
        (0, 0, -1),
    };
}
