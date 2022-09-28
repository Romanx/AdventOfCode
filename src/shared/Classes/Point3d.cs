using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using PCRE;
using Shared.Helpers;

namespace Shared
{
    public record Point3d : Point, IComparable<Point3d?>
    {
        public const int NumberOfDimensions = 3;
        private static readonly PcreRegex regex = new(@"([-+]?[0-9]+),\s*([-+]?[0-9]+),\s*([-+]?[0-9]+)", PcreOptions.Compiled);

        public Point3d(ImmutableArray<int> dimensions) : base(dimensions)
        {
        }

        public Point3d(int row, int column, int level) : base(ImmutableArray.Create(row, column, level))
        {
        }

        public Point3d(Point original) : base(original)
        {
        }

        public int Row => Dimensions[0];

        public int Column => Dimensions[1];

        public int Level => Dimensions[2];

        public int X => Row;

        public int Y => Column;

        public int Z => Level;

        public static Point3d Origin { get; } = new(0, 0, 0);

        public static Point3d operator +(Point3d left, Point3d right)
            => new(left.Row + right.Row, left.Column + right.Column, left.Level + right.Level);

        public static Point3d Parse(ReadOnlySpan<char> span)
        {
            var match = regex.Match(span);
            if (match.Success)
            {
                var x = int.Parse(match.Groups[1].Value);
                var y = int.Parse(match.Groups[2].Value);
                var z = int.Parse(match.Groups[3].Value);

                return new Point3d(x, y, z);
            }

            throw new InvalidOperationException($"Unable to parse Point3d from '{span}'");
        }

        public static Point3d operator +(Point3d point, Direction direction)
            => AddInDirection(point, direction, 1);

        public static Point3d operator -(Point3d left, Point3d right)
            => new(left.Row - right.Row, left.Column - right.Column, left.Level - right.Level);

        public IEnumerable<Point3d> GetNeighboursInDistance(int distance)
            => PointHelpers.GetNeighboursInDistance(this, distance, dim => new(dim.ToImmutableArray()));

        public static Point3d AddInDirection(Point3d point, Direction direction, int count)
        {
            return direction.DirectionType switch
            {
                DirectionType.North => point + (0, -count, 0),
                DirectionType.NorthEast => point + (count, -count, 0),
                DirectionType.NorthWest => point + (-count, -count, 0),
                DirectionType.East => point + (count, 0, 0),
                DirectionType.South => point + (0, count, 0),
                DirectionType.SouthEast => point + (count, count, 0),
                DirectionType.SouthWest => point + (-count, count, 0),
                DirectionType.West => point + (-count, 0, 0),
                _ => point
            };
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public int CompareTo(Point3d? other) => other is null
            ? -1
            : (other.Row, other.Column, other.Level).CompareTo((Row, Column, Level));

        public static implicit operator Point3d((int X, int Y, int Z) i) => new(i.X, i.Y, i.Z);

        public static implicit operator Point2d(Point3d point) => new(point.X, point.Y);

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Point3d? other) => base.Equals(other);

        public override string ToString() => base.ToString();
    }
}
