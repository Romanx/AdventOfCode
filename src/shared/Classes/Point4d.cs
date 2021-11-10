using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Shared.Helpers;

namespace Shared
{
    public record Point4d : Point, IComparable<Point4d?>
    {
        public static int NumberOfDimensions = 4;

        public Point4d(ImmutableArray<int> dimensions) : base(dimensions)
        {
        }

        public Point4d(int row, int column, int level, int w) : base(ImmutableArray.Create(row, column, level, w))
        {
        }

        public Point4d(Point original) : base(original)
        {
        }

        public int Row => Dimensions[0];

        public int Column => Dimensions[1];

        public int Level => Dimensions[2];

        public int W => Dimensions[3];

        public int X => Row;

        public int Y => Column;

        public int Z => Level;

        public static Point4d Origin { get; } = new(0, 0, 0, 0);

        public static Point4d operator +(Point4d left, Point4d right)
            => new(left.Row + right.Row, left.Column + right.Column, left.Level + right.Level, left.W - right.W);

        public static Point4d operator -(Point4d left, Point4d right)
            => new(left.Row - right.Row, left.Column - right.Column, left.Level - right.Level, left.W - right.W);

        public IEnumerable<Point4d> GetNeighboursInDistance(int distance)
            => PointHelpers.GetNeighboursInDistance(this, distance, dim => new(dim.ToImmutableArray()));

        public int CompareTo(Point4d? other) => other is null
            ? -1
            : (other.Row, other.Column, other.Level, other.W).CompareTo((Row, Column, Level, W));

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Point3d? other) => base.Equals(other);

        public static explicit operator Point4d(Point3d p) => new(ConvertToPoint(p.Dimensions, NumberOfDimensions));

        public override string ToString() => $"[{X}, {Y}, {Z}, {W}]";
    }
}
