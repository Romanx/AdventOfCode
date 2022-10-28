using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PCRE;

namespace Shared
{
    public readonly record struct Point4d :
        IComparable<Point4d>,
        IParsable<Point4d>,
        ISpanParsable<Point4d>,
        IPoint
    {
        private static readonly PcreRegex regex = new(@"([-+]?[0-9]+),\s*([-+]?[0-9]+),\s*([-+]?[0-9]+),\s*([-+]?[0-9]+)", PcreOptions.Compiled);

        public Point4d(int row, int column, int level, int w)
        {
            Row = row;
            Column = column;
            Level = level;
            W = w;
        }

        public int Row { get; }

        public int Column { get; }

        public int Level { get; }

        public int W { get; }

        public int X => Row;

        public int Y => Column;

        public int Z => Level;

        public IEnumerable<Point4d> GetNeighboursInDistance(int distance)
            => PointHelpers.GetNeighboursInDistance(
                this,
                distance,
                static dim =>
                {
                    var arr = dim.ToArray();
                    return new(arr[0], arr[1], arr[2], arr[3]);
                });

        public int CompareTo(Point4d other) =>
            (other.Row, other.Column, other.Level, other.W).CompareTo((Row, Column, Level, W));

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            3 => W,
            _ => throw new InvalidOperationException($"Does not have a dimension at '{index}'")
        };

        public void GetDimensions(Span<int> dimensions)
        {
            dimensions[0] = X;
            dimensions[1] = Y;
            dimensions[2] = Z;
            dimensions[3] = W;
        }

        public override string ToString() => $"[{X}, {Y}, {Z}, {W}]";

        public static Point4d Origin { get; } = new(0, 0, 0, 0);

        public static int DimensionCount { get; } = 4;

        public static Point4d operator +(Point4d left, Point4d right)
            => new(left.Row + right.Row, left.Column + right.Column, left.Level + right.Level, left.W - right.W);

        public static Point4d operator -(Point4d left, Point4d right)
            => new(left.Row - right.Row, left.Column - right.Column, left.Level - right.Level, left.W - right.W);

        public static explicit operator Point4d(Point3d p) => new(p.Row, p.Column, p.Level, 0);

        public static Point4d Parse(string s, IFormatProvider? provider = null)
            => Parse(s.AsSpan(), provider);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point4d result)
            => TryParse(s.AsSpan(), provider, out result);

        public static Point4d Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            if (TryParse(s, provider, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Unable to parse Point4d from '{s}'");
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point4d result)
        {
            var match = regex.Match(s);
            if (match.Success)
            {
                var x = int.Parse(match.Groups[1].Value);
                var y = int.Parse(match.Groups[2].Value);
                var z = int.Parse(match.Groups[3].Value);
                var w = int.Parse(match.Groups[4].Value);

                result = new Point4d(x, y, z, w);
                return true;
            }

            result = Origin;
            return false;
        }
    }
}
