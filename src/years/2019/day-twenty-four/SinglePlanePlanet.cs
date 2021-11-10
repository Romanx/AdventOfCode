namespace DayTwentyFour2019
{
    public static class SinglePlanePlanet
    {
        public static ImmutableHashSet<Point2d> Step(ImmutableHashSet<Point2d> bugs)
        {
            var result = ImmutableHashSet.CreateBuilder<Point2d>();

            foreach (var point in PointsInSpace(5, 5))
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

            var adjacent = new[]
            {
                Point2d.AddInDirection(point, Direction.North, 1),
                Point2d.AddInDirection(point, Direction.East, 1),
                Point2d.AddInDirection(point, Direction.South, 1),
                Point2d.AddInDirection(point, Direction.West, 1),
            };

            foreach (var neighbour in adjacent)
            {
                if (bugs.Contains(neighbour))
                {
                    count++;
                }
            }

            return count;
        }

        private static IEnumerable<Point2d> PointsInSpace(int height, int width)
        {
            for (var column = 0; column < height; column++)
            {
                for (var row = 0; row < width; row++)
                {
                    yield return new Point2d(row, column);
                }
            }
        }

        public static long CalculateBiodiversity(ImmutableHashSet<Point2d> bugs)
        {
            long result = 0;
            var count = 0;
            for (var column = 0; column < 5; column++)
            {
                for (var row = 0; row < 5; row++)
                {
                    if (bugs.Contains((row, column)))
                    {
                        result += (long)Math.Pow(2, count);
                    }
                    count++;
                }
            }

            return result;
        }
    }
}
