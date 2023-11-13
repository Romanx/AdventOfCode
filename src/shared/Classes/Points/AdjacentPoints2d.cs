using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared;

public readonly struct AdjacentPoints2d : IEnumerable<Point2d>
{
    private readonly Point2d?[] adjacentPoints;
    private readonly AdjacencyType adjacencyType;

    public AdjacentPoints2d(Point2d centre, ISet<Point2d>? points)
        : this(centre, points, AdjacencyType.Cardinal)
    {
    }

    public AdjacentPoints2d(
        Point2d centre,
        ISet<Point2d>? points,
        AdjacencyType adjacencyType)
    {
        Centre = centre;
        this.adjacencyType = adjacencyType;
        adjacentPoints = new Point2d?[8];
        var count = 0;

        var directions = adjacencyType is AdjacencyType.Cardinal
            ? Directions.CardinalDirections
            : Directions.All;

        if (points is not null)
        {
            foreach (var direction in directions)
            {
                var point = centre + direction;
                if (points.Contains(point))
                {
                    adjacentPoints[(int)direction.DirectionType] = point;
                    count++;
                }
            }
        }
        else
        {
            foreach (var direction in directions)
            {
                var point = centre + direction;
                adjacentPoints[(int)direction.DirectionType] = point;
            }

            count = directions.Length;
        }

        Count = count;
    }

    public AdjacentPoints2d(
        Point2d centre,
        Point2d? north = null,
        Point2d? east = null,
        Point2d? south = null,
        Point2d? west = null)
    {
        Centre = centre;
        adjacencyType = AdjacencyType.Cardinal;
        adjacentPoints = new Point2d?[8];

        adjacentPoints[(int)DirectionType.North] = north;
        adjacentPoints[(int)DirectionType.East] = east;
        adjacentPoints[(int)DirectionType.South] = south;
        adjacentPoints[(int)DirectionType.West] = west;
    }

    public Point2d Centre { get; }

    public Point2d? North => adjacentPoints[(int)DirectionType.North];
    public Point2d? NorthEast => adjacentPoints[(int)DirectionType.NorthEast];
    public Point2d? NorthWest => adjacentPoints[(int)DirectionType.NorthWest];
    public Point2d? East => adjacentPoints[(int)DirectionType.East];
    public Point2d? South => adjacentPoints[(int)DirectionType.South];
    public Point2d? SouthEast => adjacentPoints[(int)DirectionType.SouthEast];
    public Point2d? SouthWest => adjacentPoints[(int)DirectionType.SouthWest];
    public Point2d? West => adjacentPoints[(int)DirectionType.West];

    public int Count { get; }

    public bool Contains(Point2d point)
        => Array.IndexOf(adjacentPoints, point) >= 0;

    public Point2d? this[Direction dir] => dir.DirectionType switch
    {
        DirectionType.North => North,
        DirectionType.NorthEast => NorthEast,
        DirectionType.NorthWest => NorthWest,
        DirectionType.East => East,
        DirectionType.South => South,
        DirectionType.SouthEast => SouthEast,
        DirectionType.SouthWest => SouthWest,
        DirectionType.West => West,
        _ => throw new InvalidOperationException()
    };

    public IEnumerator<Point2d> GetEnumerator()
    {
        var directions = adjacencyType is AdjacencyType.Cardinal
            ? Directions.CardinalDirections
            : Directions.All;

        foreach (var direction in directions)
        {
            var point = adjacentPoints[(int)direction.DirectionType];
            if (point is not null)
                yield return point.Value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public enum AdjacencyType
{
    All,
    Cardinal
}
