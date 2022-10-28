using System.Text;

namespace DayTwentyFour2019
{
    public static class MultiPlanePlanet
    {
        private static Point2d Center { get; } = new Point2d(2, 2);

        public static ImmutableHashSet<Point3d> Run(int times, ImmutableHashSet<Point3d> bugs)
        {
            for (var i = 0; i < times; i++)
            {
                bugs = Step(bugs);
            }

            return bugs;
        }

        public static ImmutableHashSet<Point3d> Step(ImmutableHashSet<Point3d> bugs)
        {
            var counts = new Dictionary<Point3d, int>();
            foreach (var position in bugs)
            {
                foreach (var neighbor in GetAdjacentPoints(position))
                {
                    counts.AddOrUpdate(
                        neighbor,
                        1,
                        static (_, num) => num + 1);
                }
            }

            var result = ImmutableHashSet.CreateBuilder<Point3d>();

            foreach (var (position, count) in counts)
            {
                var live = bugs.Contains(position)
                    ? count == 1
                    : (count == 1 || count == 2);

                if (live)
                {
                    result.Add(position);
                }
            }

            return result.ToImmutable();
        }

        internal static void Print(ImmutableHashSet<Point3d> bugs)
        {
            var builder = new StringBuilder();
            var depths = bugs
                .Select(b => b.Z)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var depth in depths)
            {
                Print(builder, depth, bugs);
                builder.AppendLine();
            }

            Console.WriteLine(builder.ToString());

            static void Print(StringBuilder builder, int level, ImmutableHashSet<Point3d> bugs)
            {
                var count = 0;
                builder.AppendLine($"Depth {level}:");
                for (var column = 0; column < 5; column++)
                {
                    for (var row = 0; row < 5; row++)
                    {
                        if (row == 2 && column == 2)
                        {
                            builder.Append('?');
                        }
                        else
                        {
                            if (bugs.Contains((row, column, level)))
                            {
                                count++;
                                builder.Append('#');
                            }
                            else
                            {
                                builder.Append('.');
                            }
                        }
                    }
                    builder.AppendLine();
                }
                builder.AppendLine($"Count {count}");
            }
        }

        private static IEnumerable<Point3d> GetAdjacentPoints(Point3d point)
        {
            var adjacentPoints = new (Direction Direction, Point3d Point)[]
            {
                (Direction.North, point + Direction.North),
                (Direction.East, point + Direction.East),
                (Direction.South, point + Direction.South),
                (Direction.West, point + Direction.West),
            };

            foreach (var (direction, adjacent) in adjacentPoints)
            {
                if ((0..4).Contains(adjacent.Row) is false || (0..4).Contains(adjacent.Column) is false)
                {
                    yield return (Center + direction).Z(point.Z - 1);
                }
                else if (adjacent is { Row: 2, Column: 2 })
                {
                    var childLevel = point.Z + 1;
                    var sidePoints = direction.DirectionType switch
                    {
                        DirectionType.North => Enumerable.Range(0, 5).Select(row => new Point3d(row, 4, childLevel)),
                        DirectionType.East => Enumerable.Range(0, 5).Select(column => new Point3d(0, column, childLevel)),
                        DirectionType.South => Enumerable.Range(0, 5).Select(row => new Point3d(row, 0, childLevel)),
                        DirectionType.West => Enumerable.Range(0, 5).Select(column => new Point3d(4, column, childLevel)),
                        _ => throw new InvalidOperationException(),
                    };
                    foreach (var p in sidePoints)
                        yield return p;
                }
                else
                {
                    yield return adjacent;
                }
            }
        }
    }
}
