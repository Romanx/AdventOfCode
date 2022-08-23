using MoreLinq;

namespace DaySeventeen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 17), "No Such Thing as Too Much");

        public void PartOne(IInput input, IOutput output)
        {
            var total = 150;
            var containers = input.Lines.Ints();

            var combinations = containers
                .Subsets()
                .Where(subset => subset.Sum() == total)
                .Count();

            output.WriteProperty("Number of Combinations", combinations);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var total = 150;
            var containers = input.Lines.Ints();

            var minimumContainers = containers
                .Subsets()
                .Where(subset => subset.Sum() == total)
                .Min(subset => subset.Count);

            var minimalContainerCombinations = containers
                .Subsets(minimumContainers)
                .Where(subset => subset.Sum() == total)
                .Count();

            output.WriteProperty("Number of Combinations", minimalContainerCombinations);
        }
    }
}
