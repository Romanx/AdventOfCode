using System;
using System.Collections.Generic;
using MoreLinq;

namespace Shared.Grid
{
    public readonly record struct DimensionRange(int Min, int Max) : IEnumerable<int>
    {
        public int Size { get; } = Max - Min + 1;

        public bool Intersects(DimensionRange other)
            => Min <= other.Max && Max >= other.Min;

        public DimensionRange Intersect(DimensionRange other)
            => new(Math.Max(Min, other.Min), Math.Min(Max, other.Max));

        public bool Contains(int value)
            => Min <= value && Max >= value;

        public IEnumerator<int> GetEnumerator() => MoreEnumerable.Sequence(Min, Max).GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
