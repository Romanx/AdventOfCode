using System.Diagnostics.CodeAnalysis;

namespace DayFour2022;

readonly record struct Assignment(NumberRange<uint> First, NumberRange<uint> Second) : ISpanParsable<Assignment>
{
    public bool HasWholeOverlap
        => First.Contains(Second) || Second.Contains(First);

    public bool HasPartialOverlap => First.Intersects(Second);

    public static Assignment Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        var split = s.IndexOf(',');
        var first = NumberRange<uint>.Parse(s[..split]);
        var second = NumberRange<uint>.Parse(s[(split + 1)..]);

        return new Assignment(first, second);
    }

    public static Assignment Parse(string s, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Assignment result)
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

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Assignment result)
        => TryParse(s.AsSpan(), provider, out result);
}
