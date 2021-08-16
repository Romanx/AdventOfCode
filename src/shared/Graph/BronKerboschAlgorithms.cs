using System;

namespace Shared.Graph
{
    public static class BronKerboschAlgorithms
    {
        public static BronKerbosch<T> BronKerbosch<T>(this IVertexGraph<T> graph)
            where T : notnull, IEquatable<T>
            => new(graph);
    }
}
