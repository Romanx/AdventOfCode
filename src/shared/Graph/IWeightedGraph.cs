using System;

namespace Shared.Graph
{
    public interface IWeightedGraph<T> : IGraph<T>
        where T : IEquatable<T>
    {
        int Cost(T nodeA, T nodeB);
    }
}
