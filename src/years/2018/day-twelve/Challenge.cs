using static MoreLinq.Extensions.WindowExtension;

namespace DayTwelve2018;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 12), "Subterranean Sustainability");

    public override void PartOne(IInput input, IOutput output)
    {
        var (initialState, rules) = input.Parse();

        var (state, plants) = MutatePlants(initialState, rules)
            .Skip(19)
            .First();

        output.WriteProperty("Current state", state);
        output.WriteProperty("Number of plants", plants);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var (initialState, rules) = input.Parse();

        const long TargetGeneration = 50_000_000_000;
        var previousDiff = 0L;
        var previousSize = 0L;
        var generation = 0;

        _ = MutatePlants(initialState, rules).SkipWhile(result =>
        {
            var (state, plants) = result;
            var thisDiff = plants - previousSize;
            if (thisDiff != previousDiff)
            {
                // Still Changing
                previousDiff = thisDiff;
                previousSize = plants;
                generation++;
                return true;
            }
            else
            {
                // We've found it stop skipping
                return false;
            }
        }).First();

        var result = previousSize + (previousDiff * (TargetGeneration - generation));

        output.WriteProperty("Generation", generation);
        output.WriteProperty("Size at generation", result);
    }

    private static IEnumerable<(string State, long Plants)> MutatePlants(string initalState, ImmutableHashSet<string> rules)
    {
        var zeroIndex = 0;
        var currentState = initalState;

        while (true)
        {
            while (currentState.StartsWith(".....") is false)
            {
                currentState = $".{currentState}";
                zeroIndex++;
            }

            while (currentState.EndsWith(".....") is false)
            {
                currentState = $"{currentState}.";
            }

            currentState = new string(currentState
                .Window(5)
                .Select(window => new string(window.ToArray()))
                .Select(window => rules.Contains(window) ? '#' : '.')
                .ToArray());

            // Because there are two positions to the left of 
            // the first real center that were not directly evaluated
            zeroIndex -= 2;

            yield return (currentState, SumIndexesFrom(currentState, zeroIndex));
        }

        static long SumIndexesFrom(ReadOnlySpan<char> input, int zero)
        {
            long total = 0;
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] is '#')
                {
                    total += (i - zero);
                }
            }

            return total;
        }
    }
}

internal static class ParseExtensions
{
    public static (string InitialState, ImmutableHashSet<string> Rules) Parse(this IInput input)
    {
        var lines = input.Lines.AsArray();

        var initalState = lines[0][15..];

        return (initalState, ParseRules(lines.AsSpan()[1..]));
    }

    private static ImmutableHashSet<string> ParseRules(ReadOnlySpan<string> lines)
    {
        var result = ImmutableHashSet.CreateBuilder<string>();

        foreach (var line in lines)
        {
            if (line.EndsWith('#'))
            {
                result.Add(line[..5]);
            }
        }

        return result.ToImmutable();
    }
}
