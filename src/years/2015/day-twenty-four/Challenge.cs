using static MoreLinq.Extensions.SubsetsExtension;

namespace DayTwentyFour2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 24), "It Hangs in the Balance");

        public override void PartOne(IInput input, IOutput output)
        {
            var vals = input.Parse();

            var group = FindSmallestFirstGroup(vals, 3);

            output.WriteProperty("Smallest First Group", string.Join(" ", group));
            output.WriteProperty("QE", Product(group));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var vals = input.Parse();

            var group = FindSmallestFirstGroup(vals, 4);

            output.WriteProperty("Smallest First Group", string.Join(" ", group));
            output.WriteProperty("QE", Product(group));
        }

        private static IList<int> FindSmallestFirstGroup(
            IEnumerable<int> input,
            int numberOfGroups)
        {
            var groupSize = input.Sum() / numberOfGroups;
            var found = false;
            var size = 1;

            var group = Enumerable.Empty<IList<int>>();
            while (found is false)
            {
                group = input.Subsets(size).Where(s => s.Sum() == groupSize);
                found = group.Any();
                size++;
            }

            return group
                .MinBy(s => Product(s))!;
        }

        static long Product(IEnumerable<int> iter) => iter
            .Select(i => (long)i)
            .Aggregate((a, b) => a * b);
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<int> Parse(this IInput input)
            => input.Lines.Ints().ToImmutableArray();
    }
}
