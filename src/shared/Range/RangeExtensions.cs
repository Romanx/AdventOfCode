using System;
using System.Diagnostics;

namespace Shared
{
    public static class RangeExtensions
    {
        public static Range FirstHalf(this Range range)
            => new(range.Start, range.MidPoint());

        public static Range LastHalf(this Range range)
            => new(range.MidPoint() + 1, range.End);

        public static int MidPoint(this Range range)
            => (int)((range.End.Value + (long)range.Start.Value) / 2);

        public static bool Contains(this Range range, int value) => value >= range.Start.Value && value <= range.End.Value;

        public static int BinarySearch(this Range range, Func<int, BinarySearchResult> func)
        {
            Debug.Assert(range.Start.IsFromEnd is false, "Cannot binary search if start is from end");
            Debug.Assert(range.End.IsFromEnd is false, "Cannot binary search if end is from end");

            while (range.Start.Value != range.End.Value)
            {
                var midpoint = range.MidPoint();
                var result = func(midpoint);

                if (result == BinarySearchResult.Lower)
                {
                    range = range.FirstHalf();
                }
                else if (result == BinarySearchResult.Upper)
                {
                    range = range.LastHalf();
                }
            }

            return range.Start.Value;
        }
    }

    public enum BinarySearchResult
    {
        Upper,
        Lower
    }
}
