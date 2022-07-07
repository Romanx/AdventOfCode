using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared.Graph
{
    public interface IVertexGraph<T> : IGraph<T>
        where T : IEquatable<T>
    {
        ImmutableArray<T> Vertexes { get; }
    }
}
