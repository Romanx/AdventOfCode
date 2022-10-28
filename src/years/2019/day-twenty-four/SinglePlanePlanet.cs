using System.Collections.Specialized;
using CommunityToolkit.HighPerformance.Helpers;
using Shared.Grid;

namespace DayTwentyFour2019
{
    public static class SinglePlanePlanet
    {
        private static readonly Area2d Area = Area2d.Create(5, 5);

        public static Plane Step(Plane plane)
        {
            var next = new Plane(Area);

            foreach (var point in Area.Items)
            {
                var neighbors = PointHelpers.GetDirectNeighbours(point)
                    .Where(Area.Contains)
                    .Count(plane.At);

                var active = plane.At(point)
                    ? neighbors is 1
                    : neighbors is 1 or 2;

                if (active)
                {
                    next.Set(point, active);
                }
            }

            return next;
        }
    }

    public record struct Plane : IEquatable<Plane>
    {
        private readonly int yBoundary;
        ulong representation;

        public Plane(Area2d area)
        {
            yBoundary = area.Width - 1;
            representation = 0;
        }

        public void Set(Point2d point, bool value)
        {
            var idx = CalculateIndex(point);
            BitHelper.SetFlag(ref representation, idx, value);
        }

        public void SetAll(IEnumerable<Point2d> points, bool value)
        {
            foreach (var point in points)
            {
                Set(point, value);
            }
        }

        public bool At(Point2d point)
            => BitHelper.HasFlag(representation, CalculateIndex(point));

        public ulong Biodiversity => representation;

        public bool Equals(Plane other)
            => representation == other.representation;

        public override int GetHashCode()
            => representation.GetHashCode();

        public string ToString(Area2d area)
        {
            var points = new HashSet<Point2d>();
            foreach (var point in area.Items)
            {
                if (At(point))
                {
                    points.Add(point);
                }
            }

            return GridPrinter.Print(points, '#');
        }

        private int CalculateIndex(Point2d point)
            => point.X + (point.Y * (yBoundary + 1));
    }
}
