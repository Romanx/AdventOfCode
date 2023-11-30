using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared.Graph;

public class GridGraph<TValue>(ImmutableDictionary<Point2d, TValue> map) : IWeightedGraph<Point2d>
{
    public ImmutableDictionary<Point2d, TValue> Map { get; } = map;

    public IEnumerable<Point2d> Neighbours(Point2d current)
    {
        foreach (var neighbour in current.GetNeighbours(AdjacencyType.Cardinal))
        {
            if (Map.TryGetValue(neighbour, out var value) && IsValidNeighbours(current, Map[current], neighbour, value))
            {
                yield return neighbour;
            }
        }
    }

    public virtual int Cost(Point2d nodeA, Point2d nodeB)
        => 1;

    public virtual bool IsValidNeighbours(
        Point2d current,
        TValue currentValue,
        Point2d neighbour,
        TValue neighbourValue)
        => true;
}

public sealed class SimpleGridGraph<TValue>(
    ImmutableDictionary<Point2d, TValue> map,
    SimpleGridGraph<TValue>.IsValidNeighboursCheck check) : GridGraph<TValue>(map)
{
    private readonly SimpleGridGraph<TValue>.IsValidNeighboursCheck check = check;

    public delegate bool IsValidNeighboursCheck(
        Point2d current,
        TValue currentValue,
        Point2d neighbour,
        TValue neighbourValue);

    public override bool IsValidNeighbours(Point2d current, TValue currentValue, Point2d neighbour, TValue neighbourValue)
        => check(current, currentValue, neighbour, neighbourValue);
}

public static class GridGraphExtensions
{
    public static GridGraph<TValue> ToGridGraph<TValue>(
        this ImmutableDictionary<Point2d, TValue> grid,
        SimpleGridGraph<TValue>.IsValidNeighboursCheck? check = null)
    {
        return new SimpleGridGraph<TValue>(grid, check ?? IsValidNeighbours);

        static bool IsValidNeighbours(Point2d current, TValue currentValue, Point2d neighbour, TValue neighbourValue)
            => true;
    }
}
