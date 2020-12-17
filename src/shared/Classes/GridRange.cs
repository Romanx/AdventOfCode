using System.Collections.Generic;
using MoreLinq;

namespace Shared
{
    public record GridRange(int Min, int Max) : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => MoreEnumerable.Sequence(Min, Max).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
