namespace DaySeven2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 7), "The Treachery of Whales");

    public override void PartOne(IInput input, IOutput output)
    {
        var crabs = input.Content.AsInts();

        var min = crabs.Min();
        var max = crabs.Max();

        var leastFuelUsed = Enumerable.Range(min, max)
            .Min(position => CalculateTotalCost(position, crabs));

        output.WriteProperty("Least Fuel Used", leastFuelUsed);

        static int CalculateTotalCost(int target, IEnumerable<int> crabs)
        {
            return crabs
                .Sum(crab => Math.Abs(crab - target));
        }
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var crabs = input.Content.AsInts();

        var min = crabs.Min();
        var max = crabs.Max();

        var leastFuelUsed = Enumerable.Range(min, max)
            .Min(position => CalculateTotalCost(position, crabs));

        output.WriteProperty("Least Fuel Used", leastFuelUsed);

        static int CalculateTotalCost(int target, IEnumerable<int> crabs)
        {
            return crabs
                .Sum(crab =>
                {
                    var distance = Math.Abs(crab - target);

                    return (distance * (distance + 1)) / 2;
                });
        }
    }
}
