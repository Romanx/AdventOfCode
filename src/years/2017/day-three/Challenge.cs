using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Shared;
using Shared.Grid;
using Shared.Helpers;

namespace DayThree2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 3), "Spiral Memory");

        public override void PartOne(IInput input, IOutput output)
        {
            var target = input.Content.AsInt();

            var end = SpiralPointGenerator().ElementAt(target - 1);

            output.WriteProperty($"Distance from (0, 0) to {end}", PointHelpers.ManhattanDistance(Point2d.Origin, end));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var target = input.Content.AsInt();

            var value = SpiralSums().First(x => x > target);

            output.WriteProperty($"Value greater than {target}", value);

            static IEnumerable<int> SpiralSums()
            {
                var dict = new Dictionary<Point2d, int>();
                foreach (var point in SpiralPointGenerator())
                {
                    dict[point] = point
                        .GetAllNeighbours()
                        .Select(n => dict.TryGetValue(n, out var val) ? (int?)val : null)
                        .Where(n => n != null)
                        .DefaultIfEmpty(1)
                        .Sum()!.Value;

                    yield return dict[point];
                }
            }
        }

        static IEnumerable<Point2d> SpiralPointGenerator() => Algorithm.SpiralPointGenerator(Point2d.Origin, GridDirection.Right, Rotation.CounterClockwise);
    }
}
