using System;
using System.Collections.Generic;
using MoreLinq;

namespace Shared.Grid
{
    public record DimensionRange(int Min, int Max) : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator() => MoreEnumerable.Sequence(Min, Max).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count { get; } = Max - Min + 1;

        public static implicit operator DimensionRange(Range range)
        {
            if (range.Start.IsFromEnd || range.End.IsFromEnd)
            {
                throw new InvalidOperationException("Can't be a dimension range if either index is from the end.");
            }

            return new(range.Start.Value, range.End.Value);
        }
    }
}
