using System;
using System.Numerics;

namespace System
{
    public static class SpanExtensions
    {
        public static bool IsDistinct(this ReadOnlySpan<char> input)
        {
            Span<int> counts = stackalloc int[256];

            for (var i = 0; i < input.Length; i++)
            {
                var item = input[i];
                ref var current = ref counts[item];
                if (current is 1)
                {
                    return false;
                }
                current++;
            }

            return true;
        }

        public static ReadOnlySpan<char> Distinct(this ReadOnlySpan<char> input)
        {
            Span<int> counts = stackalloc int[256];

            for (var i = 0; i < input.Length; i++)
            {
                var item = input[i];
                counts[item]++;
            }

            Span<char> result = new char[input.Length];
            var total = 0;
            for (var i = 0; i < counts.Length; i++)
            {
                if (counts[i] is 1)
                {
                    result[total] = (char)counts[i];
                    total++;
                }
            }

            return result[..total];
        }

        public static T Max<T>(this Span<T> span)
            where T : INumber<T>
            => Max((ReadOnlySpan<T>)span);

        public static T Max<T>(this ReadOnlySpan<T> span)
            where T : INumber<T>
        {
            if (span.Length is 0)
            {
                throw new InvalidOperationException("Cannot get max item of an empty span");
            }

            var current = span[0];
            for (var i = 1; i < span.Length; i++)
            {
                if (span[i] > current)
                {
                    current = span[i];
                }
            }

            return current;
        }

        public static bool All<T>(this Span<T> span, T item)
            where T : IEquatable<T>
            => All((ReadOnlySpan<T>)span, item);

        public static bool All<T>(this ReadOnlySpan<T> span, T item)
            where T : IEquatable<T>
        {
            for (var i = 0; i <  span.Length; i++)
            {
                if (span[i].Equals(item) is false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Any<T>(this Span<T> span, T item)
            where T : IEquatable<T>
            => Any((ReadOnlySpan<T>)span, item);

        public static bool Any<T>(this ReadOnlySpan<T> span, T item)
            where T : IEquatable<T>
        {
            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].Equals(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
