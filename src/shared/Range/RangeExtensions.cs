using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Shared
{
    public static class RangeExtensions
    {
        public static bool Contains(this Range range, int number)
            => number >= range.Start.Value && number <= range.End.Value;

        public static NumberRange<T> FirstHalf<T>(this NumberRange<T> range) where T : INumber<T>
            => new(range.Start, range.MidPoint());

        public static NumberRange<T> LastHalf<T>(this NumberRange<T> range) where T : INumber<T>
            => new(range.MidPoint() + T.One, range.End);

        public static T MidPoint<T>(this NumberRange<T> range) where T : INumber<T>
            => (range.End + range.Start) / T.CreateChecked(2);

        public static bool Contains<T>(this NumberRange<T> range, T value) where T : INumber<T>
            => value >= range.Start && value <= range.End;

        public static T BinarySearch<T>(this NumberRange<T> range, Func<T, BinarySearchResult> func) where T : INumber<T>
            => BinarySearch(range, func, null);

        public static T BinarySearch<T>(this NumberRange<T> range, Func<T, BinarySearchResult> func, Action<T, BinarySearchResult>? stepLog)
            where T : INumber<T>
        {
            while (range.Start != range.End)
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

            return range.Start;
        }

        public static RangeEnumerator<T> GetEnumerator<T>(this NumberRange<T> range)
            where T : INumber<T>
        {
            return new RangeEnumerator<T>(range.Start, range.End);
        }

        public static EnumerableRange<T> ToEnumerable<T>(this NumberRange<T> range)
            where T : INumber<T>
        {
            return new EnumerableRange<T>(range);
        }

        public struct EnumerableRange<T> : IEnumerable<T>
            where T : INumber<T>
        {
            public NumberRange<T> Range { get; private set; }

            public EnumerableRange(NumberRange<T> range)
            {
                Range = range;
            }

            public IEnumerator<T> GetEnumerator()
            {
                var enumerator = Range.GetEnumerator();
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct RangeEnumerator<T> : IEnumerator<T>
            where T : INumber<T>
        {
            private readonly T _end;
            private T _current;

            public RangeEnumerator(T start, T end)
            {
                _current = start - T.One; // - 1 fixes a bug in the original code
                _end = end;
            }

            public T Current => _current;

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
