using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PCRE;

namespace Shared
{
    public readonly partial record struct Point3d :
        IComparable<Point3d>,
        IParsable<Point3d>,
        ISpanParsable<Point3d>,
        IPoint
    {
        public Point3d(int row, int column, int level)
        {
            Row = row;
            Column = column;
            Level = level;
        }

        public int Row { get; }

        public int Column { get; }

        public int Level { get; }

        public int X => Row;

        public int Y => Column;

        public int Z => Level;

        public static Point3d Origin { get; } = new(0, 0, 0);

        public static int DimensionCount { get; } = 3;

        public static Point3d operator +(Point3d left, Point3d right)
            => new(left.Row + right.Row, left.Column + right.Column, left.Level + right.Level);

        public static Point3d operator +(Point3d point, Direction direction)
            => AddInDirection(point, direction, 1);

        public static Point3d operator -(Point3d left, Point3d right)
            => new(left.Row - right.Row, left.Column - right.Column, left.Level - right.Level);

        public IEnumerable<Point3d> GetNeighboursInDistance(int distance)
            => PointHelpers.GetNeighboursInDistance(
                this,
                distance,
                static dim =>
                {
                    var arr = dim.ToArray();
                    return new(arr[0], arr[1], arr[2]);
                });

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

        public int this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new InvalidOperationException($"Does not have a dimension at '{index}'")
        };

        public void GetDimensions(Span<int> dimensions)
        {
            dimensions[0] = X;
            dimensions[1] = Y;
            dimensions[2] = Z;
        }

        public int CompareTo(Point3d other) =>
            (other.Row, other.Column, other.Level).CompareTo((Row, Column, Level));

        public override string ToString() => $"[{X}, {Y}, {Z}]";
    }
}
