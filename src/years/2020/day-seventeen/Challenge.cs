﻿using Shared.Grid;

namespace DaySeventeen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 17), "Conway Cubes");

        public void PartOne(IInput input, IOutput output)
        {
            var space = input.ParseSpace3d();

            for (var i = 0; i < 6; i++)
            {
                space = Cycle(space);
            }

            output.WriteProperty("Active Cubes", space.Count);

            static ImmutableHashSet<Point3d> Cycle(ImmutableHashSet<Point3d> active)
            {
                var result = ImmutableHashSet.CreateBuilder<Point3d>();

                foreach (var point in PointsInSpace(active))
                {
                    var countNeighbours = CountActiveNeighbours(point, active);

                    if (active.Contains(point))
                    {
                        if (countNeighbours == 2 || countNeighbours == 3)
                        {
                            result.Add(point);
                        }
                    }
                    else if (countNeighbours == 3)
                    {
                        result.Add(point);
                    }
                }

                return result.ToImmutable();
            }

            static IEnumerable<Point3d> PointsInSpace(ImmutableHashSet<Point3d> active)
            {
                var ranges = PointHelpers.FindSpaceOfPoints(active)
                    .Select(r => new DimensionRange(r.Min - 1, r.Max + 1))
                    .ToImmutableArray();

                return PointHelpers.PointsInSpace(
                    ranges,
                    static dim =>
                    {
                        var dimensions = dim.ToArray();
                        return new Point3d(
                            dimensions[0],
                            dimensions[1],
                            dimensions[2]);
                    });
            }

            static int CountActiveNeighbours(Point3d point, ImmutableHashSet<Point3d> active)
            {
                var count = 0;
                foreach (var neighbour in point.GetNeighboursInDistance(1))
                {
                    if (active.Contains(neighbour))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var space = input.ParseSpace4d();

            for (var i = 0; i < 6; i++)
            {
                space = Cycle(space);
            }

            output.WriteProperty("Active Cubes", space.Count);

            static ImmutableHashSet<Point4d> Cycle(ImmutableHashSet<Point4d> active)
            {
                var result = ImmutableHashSet.CreateBuilder<Point4d>();

                foreach (var point in PointsInSpace(active))
                {
                    var countNeighbours = CountActiveNeighbours(point, active);

                    if (active.Contains(point))
                    {
                        if (countNeighbours == 2 || countNeighbours == 3)
                        {
                            result.Add(point);
                        }
                    }
                    else if (countNeighbours == 3)
                    {
                        result.Add(point);
                    }
                }

                return result.ToImmutable();
            }

            static IEnumerable<Point4d> PointsInSpace(ImmutableHashSet<Point4d> active)
            {
                var ranges = PointHelpers.FindSpaceOfPoints(active)
                    .Select(r => new DimensionRange(r.Min - 1, r.Max + 1))
                    .ToImmutableArray();

                return PointHelpers.PointsInSpace(
                    ranges,
                    static dim =>
                    {
                        var dimensions = dim.ToArray();
                        return new Point4d(
                            dimensions[0],
                            dimensions[1],
                            dimensions[2],
                            dimensions[3]);
                    });
            }

            static int CountActiveNeighbours(Point4d point, ImmutableHashSet<Point4d> active)
            {
                var count = 0;
                foreach (var neighbour in point.GetNeighboursInDistance(1))
                {
                    if (active.Contains(neighbour))
                    {
                        count++;
                    }
                }

                return count;
            }
        }
    }
}
