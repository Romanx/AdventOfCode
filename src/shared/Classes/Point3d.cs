using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Shared.Helpers;

namespace Shared
{
    public record Point3d : Point, IComparable<Point3d?>
    {
        public const int NumberOfDimensions = 3;

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

        public static Point3d operator -(Point3d left, Point3d right)
            => new(left.Row - right.Row, left.Column - right.Column, left.Level - right.Level);

        public IEnumerable<Point3d> GetNeighboursInDistance(int distance)
            => PointHelpers.GetNeighboursInDistance(this, distance, dim => new(dim.ToImmutableArray()));

        public int CompareTo(Point3d? other) => other is null
            ? -1
            : (other.Row, other.Column, other.Level).CompareTo((Row, Column, Level));

        public static implicit operator Point3d((int X, int Y, int Z) i) => new(i.X, i.Y, i.Z);

        public override int GetHashCode() => base.GetHashCode();

        public virtual bool Equals(Point3d? other) => base.Equals(other);

        public override string ToString() => base.ToString();
    }
}
