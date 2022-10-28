using Shared.Grid;

namespace DayTwentyFour2019
{
    public static class SinglePlanePlanet
    {
        private static readonly Area2d Area = Area2d.Create(5, 5);

        public static ImmutableHashSet<Point2d> Step(ImmutableHashSet<Point2d> bugs)
        {
            var result = ImmutableHashSet.CreateBuilder<Point2d>();

            foreach (var point in Area.Items)
            {
                var countNeighbours = CountActiveNeighbours(point, bugs);

                if (bugs.Contains(point))
                {
                    if (countNeighbours == 1)
                    {
                        result.Add(point);
                    }
                }
                else
                {
                    if (countNeighbours == 1 || countNeighbours == 2)
                    {
                        result.Add(point);
                    }
                }
            }

            return result.ToImmutable();
        }

        private static int CountActiveNeighbours(Point2d point, ImmutableHashSet<Point2d> bugs)
        {
            var count = 0;

            foreach (var neighbour in PointHelpers.GetDirectNeighbours(point))
            {
                if (bugs.Contains(neighbour))
                {
                    count++;
                }
            }

            return count;
        }

        public static long CalculateBiodiversity(ImmutableHashSet<Point2d> bugs)
        {
            long result = 0;
            var count = 0;
            
            foreach (var point in Area.Items)
            {
                if (bugs.Contains(point))
                {
                    result += (long)Math.Pow(2, count);
                }
                count++;
            }

            return result;
        }
    }
}
