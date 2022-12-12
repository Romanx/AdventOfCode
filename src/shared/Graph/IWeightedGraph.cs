using System;
using System.Numerics;

namespace Shared.Graph
{
    public interface IWeightedGraph<T> : IWeightedGraph<T, int>
        where T : IEquatable<T>
    {
    }

    public interface IWeightedGraph<T, THuristicValue> : IGraph<T>
        where T : IEquatable<T>
        where THuristicValue : INumber<THuristicValue>
    {
        THuristicValue Cost(T nodeA, T nodeB);
    }
}
