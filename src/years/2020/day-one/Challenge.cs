using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using NodaTime.Extensions;
using Shared;
using static Shared.Helpers.CollectionHelpers;

namespace DayOne2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 01), "Report Repair");

        public override void PartOne(IInput input, IOutput output)
        {
            var lines = input.AsInts();

            var items = FindNThatSumTo(lines, 2, 2020);
            var result = items.Aggregate(1, (acc, i) => acc * i);

            output.WriteProperty("Numbers", string.Join(", ", items));
            output.WriteProperty("Result", result.ToString());
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var lines = input.AsInts();

            var items = FindNThatSumTo(lines, 3, 2020);
            var result = items.Aggregate(1, (acc, i) => acc * i);

            output.WriteProperty("Numbers", string.Join(", ", items));
            output.WriteProperty("Result", result.ToString());
        }

        private static ImmutableArray<int> FindNThatSumTo(int[] lines, int subsetSize, int sum)
        {
            var subsets = FindSubsetsOfSize(lines, subsetSize);
            return subsets.First(s => s.Sum() == sum);
        }
    }

    internal static class ParsingExtensions
    {
        public static int[] AsInts(this IInput input) => input.AsLines().Select(l => int.Parse(l.Span)).ToArray();
    }
}
