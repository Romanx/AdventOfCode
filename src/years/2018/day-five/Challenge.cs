using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DayFive2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 5), "Alchemical Reduction");

    public void PartOne(IInput input, IOutput output)
    {
        var polymer = input.Content.AsString();

        var result = CollapseUntilStable(polymer);

        output.WriteProperty("Final Polymer", result);
        output.WriteProperty("Polymer length", result.Length);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var polymer = input.Content.AsString();

        var polymers = new ConcurrentBag<string>();

        AlphabetHelper.LowercaseEnumerable
            .AsParallel()
            .Select(@char =>
            {
                var updated = PcreRegex.Replace(
                    polymer,
                    $"{@char}",
                    string.Empty,
                    PcreOptions.IgnoreCase);

                if (updated.Length == polymer.Length)
                    return polymer;

                var result = CollapseUntilStable(updated);
                return result;
            })
            .ForAll(polymers.Add);

        var best = polymers
            .MinBy(p => p.Length)!;

        output.WriteProperty("Best polymer is", best);
        output.WriteProperty("Polymer length", best.Length);
    }

    private static string CollapseUntilStable(string polymer)
    {
        bool changed;
        do
        {
            (polymer, changed) = CollapsePolymers(polymer);
        } while (changed is true);

        return polymer;
    }

    private static (string Result, bool Changed) CollapsePolymers(string polymer)
    {
        HashSet<int> unitIndexes = new();

        var span = polymer.AsSpan();
        for (var i = 0; i < span.Length; i++)
        {
            if (i + 1 >= span.Length)
            {
                break;
            }

            var current = span[i];
            var next = span[i + 1];

            var same = char.ToLowerInvariant(current) == char.ToLowerInvariant(next);
            if (same && current != next)
            {
                unitIndexes.Add(i);
                i += 2;
            }
        }

        if (unitIndexes.Count == 0)
        {
            return (polymer, false);
        }

        // The new length is the current string length where we've removed two characters per polymer.
        var newLength = span.Length - (2 * unitIndexes.Count);
        var result = string.Create(newLength, (polymer, unitIndexes), static (span, state) =>
        {
            var (polymer, indexes) = state;
            var polymerSpan = polymer.AsSpan();

            var index = 0;
            for (var i = 0; i < polymerSpan.Length; i++)
            {
                if (indexes.Contains(i))
                {
                    i += 2;
                }

                if (i >= polymerSpan.Length)
                    return;

                span[index] = polymerSpan[i];
                index++;
            }
        });

        return (result, true);
    }
}
