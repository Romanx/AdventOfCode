using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Shared.Graph
{
    public sealed record Graph<T> : IVertexGraph<T>
        where T : IEquatable<T>, IComparable<T>
    {
        private readonly ImmutableDictionary<T, ImmutableArray<T>> _map;

        public Graph(ImmutableDictionary<T, ImmutableArray<T>> map)
        {
            _map = map;
            var builder = new HashSet<T>();
            builder.UnionWith(_map.Keys);
            builder.UnionWith(_map.Values.SelectMany(v => v));
            Vertexes = builder.ToImmutableArray();
        }

        public ImmutableArray<T> Vertexes { get; }

        public IEnumerable<T> Neighbours(T node) => _map.TryGetValue(node, out var neighbours)
            ? neighbours.OrderBy(c => c)
            : ImmutableArray<T>.Empty;
    }
}
