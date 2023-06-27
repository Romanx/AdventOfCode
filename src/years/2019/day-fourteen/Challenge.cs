namespace DayFourteen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 14), "Space Stoichiometry");

    public void PartOne(IInput input, IOutput output)
    {
        var reactions = input.Parse();

        var factory = new Factory(reactions);
        var fuelNeeded = factory.CalculateCost(new Chemical(1, "FUEL"), "ORE");

        output.WriteProperty("Fuel needed", fuelNeeded);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var reactions = input.Parse();
        const long target = 1_000_000_000_000;

        var factory = new Factory(reactions);

        NumberRange<int> range = 0..int.MaxValue;

        var result = range.BinarySearch(val =>
        {
            var fuelNeeded = factory.CalculateCost(new Chemical(val, "FUEL"), "ORE");

            return fuelNeeded switch
            {
                > target => BinarySearchResult.Lower,
                <= target => BinarySearchResult.Upper,
            };
        });

        // This has found the lowest value that gets the to target
        var fuelInput = result - 1;

        output.WriteProperty("Target Fuel", fuelInput);
    }
}

internal class Factory
{
    private readonly ImmutableDictionary<string, Reaction> map;

    public Factory(ImmutableArray<Reaction> reactions)
    {
        map = reactions
            .ToImmutableDictionary(k => k.Output.Type, v => v);
    }

    public long CalculateCost(Chemical target, string type)
    {
        var needed = new Dictionary<string, long>()
        {
            [target.Type] = target.Quantity,
        };
        var cache = new Dictionary<string, long>();

        Produce(ref needed, ref cache, target.Type);

        return needed[type];
    }

    private void Produce(
        ref Dictionary<string, long> needed,
        ref Dictionary<string, long> cache,
        string target)
    {
        if (map.TryGetValue(target, out var reaction) is false)
        {
            return;
        }

        var output = reaction.Output;

        var stored = cache.GetValueOrDefault(output.Type);
        var iterations = (long)Math.Ceiling(
            Math.Max(0, needed[output.Type] - stored) / (decimal)output.Quantity);

        cache.AddOrUpdate(
            output.Type,
            (_, state) => state,
            (_, val, state) => val + state,
            iterations * output.Quantity);

        foreach (var input in reaction.Inputs)
        {
            needed.AddOrUpdate(
                input.Type,
                (_, state) => state,
                (_, val, state) => val + state,
                iterations * input.Quantity);
        }

        foreach (var input in reaction.Inputs)
        {
            Produce(ref needed, ref cache, input.Type);
        }
    }
}

readonly record struct Reaction(ImmutableArray<Chemical> Inputs, Chemical Output)
{
    public override string ToString()
        => $"{string.Join(", ", Inputs)} => {Output}";
}

readonly record struct Chemical(long Quantity, string Type)
{
    public override string ToString() => $"{Quantity} {Type}";
}
