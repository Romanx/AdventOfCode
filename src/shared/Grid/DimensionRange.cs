using System.Collections.Generic;
using MoreLinq;

namespace Shared.Grid
{
    public record DimensionRange(int Min, int Max) : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => MoreEnumerable.Sequence(Min, Max).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count { get; } = Max - Min + 1;
    }
}
