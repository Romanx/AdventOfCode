using System;
using System.Collections.Generic;

namespace Shared.Graph
{
    public interface IGraph<T>
        where T : IEquatable<T>
    {
        IEnumerable<T> Neigbours(T node);
    }
}
