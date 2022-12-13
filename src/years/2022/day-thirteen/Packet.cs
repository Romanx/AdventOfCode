using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace DayThirteen2022;

readonly record struct Packet(ImmutableArray<object> Values)
    : IComparable<Packet>,
      ISpanParsable<Packet>
{
    public override string ToString()
    {
        return $"[{string.Join(",", Values.Select(ValueToString))}]";

        static string ValueToString(object value)
        {
            return value switch
            {
                int intVal => intVal.ToString(),
                ImmutableArray<object> arr => $"[{string.Join(",", arr.Select(ValueToString))}]",
                _ => "Nope",
            };
        }
    }

    public int CompareTo(Packet other)
    {
        return CompareLists(Values, other.Values);

        int CompareValues(object left, object right)
        {
            return (left, right) switch
            {
                (int l, int r)=> l.CompareTo(r),
                (ImmutableArray<object> l, ImmutableArray<object> r) => CompareLists(l, r),
                (int l, ImmutableArray<object> r) => CompareLists(ImmutableArray.Create<object>(l), r),
                (ImmutableArray<object> l, int r) => CompareLists(l, ImmutableArray.Create<object>(r)),
                _ => throw new InvalidOperationException("Whoops!")
            };
        }

        int CompareLists(ImmutableArray<object> left, ImmutableArray<object> right)
        {
            var index = 0;
            while (true)
            {
                // If the lists are the same length and no comparison makes a decision about the order, continue checking the next part of the input.
                if (index == left.Length && index == right.Length)
                {
                    return 0;
                }
                // If the left list runs out of items first, the inputs are in the right order.
                if (index == left.Length)
                {
                    return -1;
                }
                // If the right list runs out of items first, the inputs are not in the right order.
                else if (index == right.Length)
                {
                    return 1;
                }

                var leftVal = left[index];
                var rightVal = right[index];

                var result = CompareValues(leftVal, rightVal);
                if (result is not 0)
                {
                    return result;
                }

                index++;
            }
        }
    }

    public static Packet Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Unable to parse packet from input '{s}'");
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Packet result)
    {
        var copy = s;

        result = new Packet(ParseValues(ref copy));
        return true;

        static ImmutableArray<object> ParseValues(ref ReadOnlySpan<char> span)
        {
            Debug.Assert(span[0] is '[');
            var values = ImmutableArray.CreateBuilder<object>();

            span = span[1..];
            while (span.IsEmpty is false)
            {
                if (char.IsDigit(span[0]))
                {
                    var split = span.IndexOfAny(',', ']');
                    var digit = int.Parse(span[..split]);
                    values.Add(digit);
                    span = span[split] is ']'
                        ? span[split..]
                        : span[(split + 1)..];
                }
                else if (span[0] is '[')
                {
                    var inner = ParseValues(ref span);
                    values.Add(inner);
                }
                else if (span[0] is ']')
                {
                    span = span[1..];
                    break;
                }
                else if (span[0] is ',')
                {
                    span = span[1..];
                }
            }

            return values.ToImmutable();
        }
    }

    public static Packet Parse(string s, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Packet result)
        => TryParse(s.AsSpan(), provider, out result);
}
