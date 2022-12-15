using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PCRE;

namespace Shared
{
    public readonly partial record struct Point2d :
        IComparable<Point2d>,
        IParsable<Point2d>,
        ISpanParsable<Point2d>,
        IEquatable<Point2d>,
        IPoint
    {
        public static PcreRegex RegexExpression { get; } = new(@"([-+]?[0-9]+),\s*([-+]?[0-9]+)", PcreOptions.Compiled);

        public Point2d(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public int X => Row;

        public int Y => Column;

        public static Point2d Origin { get; } = new(0, 0);

        public int CompareTo(Point2d other) =>
            (Row, Column).CompareTo((other.Row, other.Column));

        public Point2d RotateAroundPivot(Point2d pivot, int angleInDegrees)
        {
            var radians = angleInDegrees * MathF.PI / 180;

            var s = MathF.Sin(radians);
            var c = MathF.Cos(radians);

            var translated = this - pivot;

            // Rotate
            var xnew = (int)MathF.Round(translated.X * c - translated.Y * s);
            var ynew = (int)MathF.Round(translated.X * s + translated.Y * c);

            return new Point2d(xnew + pivot.X, ynew + pivot.Y);
        }

        public IEnumerable<Point2d> GetAllNeighbours()
        {
            foreach (var direction in Directions.All)
            {
                yield return this + direction;
            }
        }

        public Point3d Z(int depth) => new(Row, Column, depth);

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new InvalidOperationException($"Does not have a dimension at '{index}'")
        };

        public override string ToString() => $"[{X}, {Y}]";

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public bool Equals(Point2d other)
            => X == other.X && Y == other.Y;

        public static int DimensionCount => 2;

        public void GetDimensions(Span<int> dimensions)
        {
            dimensions[0] = X;
            dimensions[1] = Y;
        }

        public static Point2d Parse(string s, IFormatProvider? provider = null)
            => Parse(s.AsSpan(), provider);

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point2d result)
            => TryParse(s.AsSpan(), provider, out result);

        public static Point2d Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            if (TryParse(s, provider, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Unable to parse Point2d from '{s}'");
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point2d result)
        {
            var match = RegexExpression.Match(s);
            if (match.Success)
            {
                var x = int.Parse(match.Groups[1].Value);
                var y = int.Parse(match.Groups[2].Value);

                result = new Point2d(x, y);
                return true;
            }

            result = Origin;
            return false;
        }

        public static implicit operator Point2d((int X, int Y) i) => new(i.X, i.Y);

        public static Point2d operator +(Point2d left, Point2d right)
            => new(left.Row + right.Row, left.Column + right.Column);

        public static Point2d operator +(Point2d left, int value)
            => new(left.Row + value, left.Column + value);

        public static Point2d operator -(Point2d left, Point2d right)
            => new(left.Row - right.Row, left.Column - right.Column);

        public static Point2d operator -(Point2d left, int value)
            => new(left.Row - value, left.Column - value);

        public static Point2d operator *(Point2d left, int multiplier)
            => new(left.Row * multiplier, left.Column * multiplier);

        public static Point2d operator /(Point2d left, int divisor)
            => new(left.Row / divisor, left.Column / divisor);

        public static Point2d operator +(Point2d point, Direction direction)
            => point + (direction, 1);

        public static Point2d operator -(Point2d point, Direction direction)
            => point - (direction, 1);

        public static Point2d operator +(Point2d point, (Direction Direction, int Count) modifier) => modifier.Direction.DirectionType switch
        {
            DirectionType.None => point,
            DirectionType.North => point + (0, -modifier.Count),
            DirectionType.NorthEast => point + (modifier.Count, -modifier.Count),
            DirectionType.NorthWest => point + (-modifier.Count, -modifier.Count),
            DirectionType.East => point + (modifier.Count, 0),
            DirectionType.South => point + (0, modifier.Count),
            DirectionType.SouthEast => point + (modifier.Count, modifier.Count),
            DirectionType.SouthWest => point + (-modifier.Count, modifier.Count),
            DirectionType.West => point + (-modifier.Count, 0),
            _ => point
        };

        public static Point2d operator -(Point2d point, (Direction Direction, int Count) modifier) => modifier.Direction.DirectionType switch
        {
            DirectionType.None => point,
            DirectionType.North => point - (0, -modifier.Count),
            DirectionType.NorthEast => point - (modifier.Count, -modifier.Count),
            DirectionType.NorthWest => point - (-modifier.Count, -modifier.Count),
            DirectionType.East => point - (modifier.Count, 0),
            DirectionType.South => point - (0, modifier.Count),
            DirectionType.SouthEast => point - (modifier.Count, modifier.Count),
            DirectionType.SouthWest => point - (-modifier.Count, modifier.Count),
            DirectionType.West => point - (-modifier.Count, 0),
            _ => point
        };

        public static bool operator <(Point2d left, Point2d right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(Point2d left, Point2d right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(Point2d left, Point2d right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(Point2d left, Point2d right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
