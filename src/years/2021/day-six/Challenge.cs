namespace DaySix2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 6), "Lanternfish");

    public override void PartOne(IInput input, IOutput output)
    {
        var fishes = input.Content
            .AsString()
            .Split(',')
            .Select(int.Parse)
            .ToImmutableArray();

        var days = 80;
        var count = SimulateDays(days, fishes);

        output.WriteProperty($"Fishes after {days} days", count);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var fishes = input.Content
               .AsString()
               .Split(',')
               .Select(int.Parse)
               .ToImmutableArray();

        var days = 256;
        var count = SimulateDays(days, fishes);

        output.WriteProperty($"Fishes after {days} days", count);
    }

    public static long SimulateDays(int days, ImmutableArray<int> fishes)
    {
        var fishCounts = new long[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        foreach (var fish in fishes)
            fishCounts[fish]++;

        for (var i = 0; i < days; i++)
        {
            // Get the fish that have offspring
            var newFish = fishCounts[0];

            // Loop through every age above zero and shuffle them down.
            for (var age = 1; age < fishCounts.Length; age++)
                fishCounts[age - 1] = fishCounts[age];

            // Set the new fish at count of 8;
            fishCounts[8] = newFish;

            // Add the fish that had offspring back to position 6.
            fishCounts[6] += newFish;
        }

        return fishCounts.Sum();
    }
}
