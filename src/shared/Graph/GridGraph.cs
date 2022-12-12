using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared.Graph;

public class GridGraph<TValue> : IWeightedGraph<Point2d>
{
    public GridGraph(
        ImmutableDictionary<Point2d, TValue> map)
    {
        Map = map;
    }
    public ImmutableDictionary<Point2d, TValue> Map { get; }

    public IEnumerable<Point2d> Neigbours(Point2d current)
    {
        foreach (var neighbour in PointHelpers.GetDirectNeighbours(current))
        {
            if (Map.TryGetValue(neighbour, out var value) && IsValidNeigbour(current, Map[current], neighbour, value))
            {
                yield return neighbour;
            }
        }
    }

    public virtual int Cost(Point2d nodeA, Point2d nodeB)
        => 1;

    public virtual bool IsValidNeigbour(
        Point2d current,
        TValue currentValue,
        Point2d neighbour,
        TValue neighbourValue)
        => true;
}

public sealed class SimpleGridGraph<TValue> : GridGraph<TValue>
{
    private readonly SimpleGridGraph<TValue>.IsValidNeigbourCheck check;

    public delegate bool IsValidNeigbourCheck(
        Point2d current,
        TValue currentValue,
        Point2d neighbour,
        TValue neighbourValue);

    public SimpleGridGraph(
        ImmutableDictionary<Point2d, TValue> map,
        IsValidNeigbourCheck check) : base(map)
    {
        this.check = check;
    }

    public override bool IsValidNeigbour(Point2d current, TValue currentValue, Point2d neighbour, TValue neighbourValue)
        => check(current, currentValue, neighbour, neighbourValue);
}

public static class GridGraphExtensions
{
    public static GridGraph<TValue> ToGridGraph<TValue>(
        this ImmutableDictionary<Point2d, TValue> grid,
        SimpleGridGraph<TValue>.IsValidNeigbourCheck? check = null)
    {
        return new SimpleGridGraph<TValue>(grid, check ?? IsValidNeigbour);

        static bool IsValidNeigbour(Point2d current, TValue currentValue, Point2d neighbour, TValue neighbourValue)
            => true;
    }
}
