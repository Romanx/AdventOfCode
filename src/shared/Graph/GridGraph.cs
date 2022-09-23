using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Shared.Helpers;

namespace Shared.Graph;

public class GridGraph<TValue> : IGraph<Point2d>
{
    protected readonly ImmutableDictionary<Point2d, TValue> map;
    private readonly Func<Point2d, TValue, bool> isValidNeighbourFunc;

    public GridGraph(
        ImmutableDictionary<Point2d, TValue> map,
        Func<Point2d, TValue, bool> isValidNeighbourFunc)
    {
        this.map = map;
        this.isValidNeighbourFunc = isValidNeighbourFunc;
    }

    public GridGraph(ImmutableDictionary<Point2d, TValue> map)
        : this(map, static (_, _) => true)
    {
    }

    public IEnumerable<Point2d> Neigbours(Point2d node)
    {
        foreach (var neighbour in PointHelpers.GetDirectNeighbours(node))
        {
            if (map.TryGetValue(neighbour, out var value) && IsValidNeigbour(neighbour, value))
            {
                yield return neighbour;
            }
        }
    }

    public virtual bool IsValidNeigbour(Point2d node, TValue value) => isValidNeighbourFunc(node, value);
}
