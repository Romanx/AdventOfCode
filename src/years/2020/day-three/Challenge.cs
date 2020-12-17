using System.Collections.Immutable;
using NodaTime;
using Shared;

namespace DayThree2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 03), "Toboggan Trajectory");

        public override void PartOne(IInput input, IOutput output)
        {
            var treeCount = CountTreesOnVector(input.As2DArray(), (3, 1));
            output.WriteProperty("Number of Trees", treeCount);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var vectors = ImmutableArray.Create((1, 1), (3, 1), (5, 1), (7, 1), (1, 2));
            var map = input.As2DArray();

            uint totalTrees = 1;
            foreach (var vector in vectors)
            {
                var treeCount = CountTreesOnVector(map, vector);
                output.WriteProperty($"Number of Trees {vector}", treeCount);
                totalTrees *= treeCount;
            }

            output.WriteProperty($"Total Trees Multiplied", totalTrees);
        }

        internal static uint CountTreesOnVector(char[,] map, Point2d moveVector)
        {
            uint treeCount = 0;
            Point2d pos = new(0, 0);
            var maxX = map.GetLength(0);
            var maxY = map.GetLength(1);

            while (pos.Column < maxY - 1)
            {
                pos += moveVector;
                var adjustedX = pos.Row % maxX;

                if (map[adjustedX, pos.Column] is '#')
                {
                    treeCount++;
                }
            }

            return treeCount;
        }
    }

    internal static class ParsingExtensions
    {
    }
}
