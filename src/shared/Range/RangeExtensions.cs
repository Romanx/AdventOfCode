using System;
using System.Collections;
using System.Collections.Generic;
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
            => BinarySearch(range, func, null);

        public static int BinarySearch(this Range range, Func<int, BinarySearchResult> func, Action<int, BinarySearchResult>? stepLog)
        {
            Debug.Assert(range.Start.IsFromEnd is false, "Cannot binary search if start is from end");
            Debug.Assert(range.End.IsFromEnd is false, "Cannot binary search if end is from end");

            while (range.Start.Value != range.End.Value)
            {
                var midpoint = range.MidPoint();
                var result = func(midpoint);

                stepLog?.Invoke(midpoint, result);

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

        public static RangeEnumerator GetEnumerator(this Range range)
        {
            if (range.Start.IsFromEnd || range.End.IsFromEnd)
            {
                throw new ArgumentException("Range cannot be from end on either axis", nameof(range));
            }

            return new RangeEnumerator(range.Start.Value, range.End.Value);
        }

        public static EnumerableRange ToEnumerable(this Range range)
        {
            return new EnumerableRange(range);
        }

        public struct EnumerableRange : IEnumerable<int>
        {
            public Range Range { get; private set; }

            public EnumerableRange(Range range)
            {
                Range = range;
            }

            public IEnumerator<int> GetEnumerator()
            {
                var enumerator = Range.GetEnumerator();
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct RangeEnumerator : IEnumerator<int>
        {
            private readonly int _end;
            private int _current;

            public RangeEnumerator(int start, int end)
            {
                _current = start - 1; // - 1 fixes a bug in the original code
                _end = end;
            }

            public int Current => _current;

            object System.Collections.IEnumerator.Current => Current;

            public bool MoveNext() => ++_current < _end;

            public void Dispose() { }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }

    public enum BinarySearchResult
    {
        Upper,
        Lower
    }
}
