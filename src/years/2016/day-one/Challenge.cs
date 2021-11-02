using System;
using System.Collections.Generic;
using NodaTime;
using Shared;
using Shared.Helpers;

namespace DayOne2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 1), "No Time for a Taxicab");

        public override void PartOne(IInput input, IOutput output)
        {
            var current = Point2d.Origin;
            var direction = Direction.North;

            foreach (var directions in input.Parse())
            {
                direction = directions.Direction switch
                {
                    'L' => direction.Left(),
                    'R' => direction.Right(),
                    _ => throw new InvalidOperationException("Invalid direction")
                };

                current = Point2d.AddInDirection(current, direction, directions.Distance);
            }

            var distance = PointHelpers.ManhattanDistance(Point2d.Origin, current);

            output.WriteProperty("Distance from start in blocks", distance);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var location = FindFirstVisitedTwice(input.Parse());

            var distance = PointHelpers.ManhattanDistance(Point2d.Origin, location);

            output.WriteProperty("Distance from start in blocks", distance);

            static Point2d FindFirstVisitedTwice(IEnumerable<(char Direction, int Distance)> direcitons)
            {
                var current = Point2d.Origin;
                var direction = Direction.North;
                var visited = new HashSet<Point2d>();

                foreach (var directions in direcitons)
                {
                    direction = directions.Direction switch
                    {
                        'L' => direction.Left(),
                        'R' => direction.Right(),
                        _ => throw new InvalidOperationException("Invalid direction")
                    };

                    for (var i = 0; i < directions.Distance; i++)
                    {
                        current += direction;
                        if (visited.Contains(current))
                        {
                            return current;
                        }
                        visited.Add(current);
                    }
                }

                throw new InvalidOperationException("No location visited twice");
            }
        }
    }

    internal static class ParseExtensions
    {
        public static IEnumerable<(char Direction, int Distance)> Parse(this IInput input)
        {
            var directions = input
                .Content
                .AsString()
                .Split(",", StringSplitOptions.TrimEntries);

            foreach (var direction in directions)
            {
                yield return (direction[0], int.Parse(direction[1..]));
            }
        }
    }
}
