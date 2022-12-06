using System;

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
    }
}
