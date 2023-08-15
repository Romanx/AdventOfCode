using System.Collections.Generic;

namespace Shared.Grid
{
    public readonly record struct Area3d(DimensionRange XRange, DimensionRange YRange, DimensionRange ZRange)
    {
        public long NumberOfPoints => 1L * XRange.Size * YRange.Size * ZRange.Size;

        public bool Intersects(Area3d other)
            => XRange.Intersects(other.XRange) && YRange.Intersects(other.YRange) && ZRange.Intersects(other.ZRange);

        public Area3d? Intersect(Area3d other)
        {
            if (Intersects(other) is false)
            {
                return null;
            }

            return new Area3d(
                XRange.Intersect(other.XRange),
                YRange.Intersect(other.YRange),
                ZRange.Intersect(other.ZRange));
        }

        public bool Contains(Point3d point) =>
            point.X < XRange.Min || point.X > XRange.Max ||
            point.Y < YRange.Min || point.Y > YRange.Max ||
            point.Z < ZRange.Min || point.Z > ZRange.Max;

        public override string ToString()
        {
            return $"[{XRange.Min},{YRange.Min},{ZRange.Min}] -> [{XRange.Max},{YRange.Max},{ZRange.Max}]";
        }

        public static Area3d Create(IEnumerable<Point3d> points)
        {
            var dimensions = PointHelpers.FindSpaceOfPoints(points);

            return new Area3d(
                dimensions[0],
                dimensions[1],
                dimensions[2]
            );
        }
    }
}
