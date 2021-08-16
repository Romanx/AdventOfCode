using System;
using System.Collections.Generic;

namespace Shared.Graph
{
    public interface IVertexGraph<T> : IGraph<T>
        where T : IEquatable<T>
    {
        IEnumerable<T> Vertexes { get; }
    }
}
