using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Shared.Helpers;

namespace Shared
{
    public record Point2d : Point, IComparable<Point2d?>
    {
        public const int NumberOfDimensions = 2;

        protected Point2d(Point original) : base(original)
        {
        }

        public Point2d(int row, int column) : this(ImmutableArray.Create(row, column))
        {
        }

        public Point2d(ImmutableArray<int> dimensions) : base(dimensions)
        {
        }

        public int Row => Dimensions[0];

        public int Column => Dimensions[1];

        public int X => Row;

        public int Y => Column;

        public static Point2d Origin { get; } = new(0, 0);

        public int CompareTo(Point2d? other) =>
            other is null
                ? -1
                : (Row, Column).CompareTo((other.Row, other.Column));

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

        public static Point2d Parse(string value)
        {
            return new Point2d(value.Split(",").Select(i => int.Parse(i)).ToImmutableArray());
        }

        public IEnumerable<Point2d> GetAllNeighbours()
            => Direction.All.Select(dir => this + dir);

        public Point3d Z(int depth) => new(Row, Column, depth);

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"({X}, {Y})";

        public static (GridRange Row, GridRange Column) FindSpaceOfPoints(IEnumerable<Point2d> points)
        {
            var spaces = PointHelpers.FindSpaceOfPoints(points, NumberOfDimensions);

            return (spaces[0], spaces[1]);
        }

        public static implicit operator Point2d((int X, int Y) i) => new(i.X, i.Y);

        public static Point2d operator +(Point2d left, Point2d right)
            => new(left.Row + right.Row, left.Column + right.Column);

        public static Point2d operator -(Point2d left, Point2d right)
            => new(left.Row - right.Row, left.Column - right.Column);

        public static Point2d operator +(Point2d point, Direction direction) => direction.DirectionType switch
        {
            DirectionType.North => point + (0, -1),
            DirectionType.NorthEast => point + (1, -1),
            DirectionType.NorthWest => point + (-1, -1),
            DirectionType.East => point + (1, 0),
            DirectionType.South => point + (0, 1),
            DirectionType.SouthEast => point + (1, 1),
            DirectionType.SouthWest => point + (-1, 1),
            DirectionType.West => point + (-1, 0),
            _ => point
        };

        public static Point2d AddInDirection(Point2d point, Direction direction, int count)
        {
            return direction.DirectionType switch
            {
                DirectionType.North => point + (0, -count),
                DirectionType.NorthEast => point + (count, -count),
                DirectionType.NorthWest => point + (-count, -count),
                DirectionType.East => point + (count, 0),
                DirectionType.South => point + (0, count),
                DirectionType.SouthEast => point + (count, count),
                DirectionType.SouthWest => point + (-count, count),
                DirectionType.West => point + (-count, 0),
                _ => point
            };
        }
    }
}
