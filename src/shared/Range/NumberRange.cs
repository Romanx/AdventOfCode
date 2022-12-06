using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CommunityToolkit.HighPerformance;
using Shared.Grid;

namespace Shared
{
    public readonly record struct NumberRange<T>(T Start, T End) : ISpanParsable<NumberRange<T>>
        where T : IBinaryNumber<T>, IParsable<T>
    {
        public bool Contains(T number) => number >= Start && number <= End;

        public bool Contains(NumberRange<T> other) => other.Start >= Start && other.End <= End;

        public bool Intersects(NumberRange<T> other)
            => Start <= other.End && End >= other.Start;

        public NumberRange<T> Intersect(NumberRange<T> other)
            => new(T.Max(Start, other.Start), T.Min(End, other.End));

        public override string ToString() => $"{Start}..{End}";

        public static NumberRange<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            var split = s.IndexOf('-');
            if (split == -1)
            {
                split = s.IndexOf("..", StringComparison.Ordinal);
            }

            if (split is -1)
            {
                throw new InvalidOperationException("Unable to parse number range");
            }

            var start = T.Parse(s[..split], provider);
            var end = T.Parse(s[(split + 1)..], provider);

            return new NumberRange<T>(start, end);
        }

        public static NumberRange<T> Parse(string s, IFormatProvider? provider = null)
            => Parse(s.AsSpan(), provider);

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberRange<T> result)
        {
            try
            {
                result = Parse(s, provider);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out NumberRange<T> result)
            => TryParse(s.AsSpan(), provider, out result);
    }
}
