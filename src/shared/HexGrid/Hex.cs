using System;
using System.Diagnostics.CodeAnalysis;

namespace Shared.HexGrid
{
    public readonly record struct Hex(Point3d Point)
    {
        public static Hex Origin { get; } = new Hex(Point3d.Origin);

        public static Hex operator +(Hex hex, HexDirection direction)
        {
            var newPoint = direction.DirectionType switch
            {
                HexDirectionType.East => hex.Point + (1, -1, 0),
                HexDirectionType.SouthEast => hex.Point + (0, -1, 1),
                HexDirectionType.SouthWest => hex.Point + (-1, 0, 1),
                HexDirectionType.West => hex.Point + (-1, 1, 0),
                HexDirectionType.NorthWest => hex.Point + (0, 1, -1),
                HexDirectionType.NorthEast => hex.Point + (1, 0, -1),
                _ => throw new NotImplementedException(),
            };

            return new(newPoint);
        }
    }

    public readonly record struct HexDirection(HexDirectionType DirectionType)
        : IParsable<HexDirection>, ISpanParsable<HexDirection>
    {
        public static HexDirection East { get; } = new HexDirection(HexDirectionType.East);
        public static HexDirection SouthEast { get; } = new HexDirection(HexDirectionType.SouthEast);
        public static HexDirection SouthWest { get; } = new HexDirection(HexDirectionType.SouthWest);
        public static HexDirection West { get; } = new HexDirection(HexDirectionType.West);
        public static HexDirection NorthWest { get; } = new HexDirection(HexDirectionType.NorthWest);
        public static HexDirection NorthEast { get; } = new HexDirection(HexDirectionType.NorthEast);

        //   \ n  /
        // nw +--+ ne
        //   /    \
        // -+      +-
        //   \    /
        // sw +--+ se
        //   / s  \
        public static HexDirection Parse(string s, IFormatProvider? provider = null)
            => Parse(s.AsSpan(), provider);

        public static HexDirection Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            if (TryParse(s, provider, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Unable to parse {s} to a HexDirection");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out HexDirection result)
            => TryParse(s.AsSpan(), provider, out result);

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out HexDirection result)
        {
            var trimmed = s.Trim();
            Span<char> upper = stackalloc char[trimmed.Length];
            trimmed.ToUpperInvariant(upper);

            (result, var success) = upper switch
            {
                "N" => (NorthWest, true),
                "NW" => (West, true),
                "NE" => (NorthEast, true),
                "S" => (SouthEast, true),
                "SW" => (SouthWest, true),
                "SE" => (East, true),
                _ => (default, false),
            };

            return success;
        }
    }


    public enum HexDirectionType
    {
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
        NorthEast
    }
}
