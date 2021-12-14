namespace DayFourteen2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 14), "Extended Polymerization");

    public override void PartOne(IInput input, IOutput output)
    {
        var (template, rules) = input.Lines.Parse();

        const int Steps = 10;

        var polymer = template;
        for (var i = 0; i < Steps; i++)
        {
            polymer = Expand(polymer, rules);
        }

        var characters = polymer.CharacterCounts
            .Select(kvp => (Char: kvp.Key, Count: kvp.Value))
            .OrderBy(kvp => kvp.Count)
            .ToArray();

        var most = characters[^1];
        var least = characters[0];

        output.WriteProperty("Most Common", $"{most.Char}: {most.Count}");
        output.WriteProperty("Least Common", $"{least.Char}: {least.Count}");

        output.WriteProperty("Quantities", $"{most.Count} - {least.Count} = {most.Count - least.Count}");
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var (template, rules) = input.Lines.Parse();

        const int Steps = 40;

        var polymer = template;
        for (var i = 0; i < Steps; i++)
        {
            polymer = Expand(polymer, rules);
        }

        var characters = polymer.CharacterCounts
            .Select(kvp => (Char: kvp.Key, Count: kvp.Value))
            .OrderBy(kvp => kvp.Count)
            .ToArray();

        var most = characters[^1];
        var least = characters[0];

        output.WriteProperty("Most Common", $"{most.Char}: {most.Count}");
        output.WriteProperty("Least Common", $"{least.Char}: {least.Count}");

        output.WriteProperty("Quantities", $"{most.Count} - {least.Count} = {most.Count - least.Count}");
    }

    private static Polymer Expand(Polymer polymer, ImmutableDictionary<(char, char), Insertion> rules)
    {
        var characters = polymer.CharacterCounts.ToBuilder();
        var newPairs = ImmutableDictionary.CreateBuilder<(char, char), long>();

        foreach (var (pair, count) in polymer.Pairs)
        {
            if (rules.TryGetValue(pair, out var insertion))
            {
                foreach (var newPair in insertion.Insertions)
                {
                    var pairCount = newPairs.GetValueOrDefault(newPair);

                    newPairs[newPair] = pairCount + count;
                }

                var characterCount = characters.GetValueOrDefault(insertion.NewCharacter);
                characters[insertion.NewCharacter] = characterCount + (1 * count);
            }
        }

        return polymer with
        {
            CharacterCounts = characters.ToImmutable(),
            Pairs = newPairs.ToImmutable(),
        };
    }
}

internal static class ParseExtensions
{
    public static (Polymer polymer, ImmutableDictionary<(char, char), Insertion> Rules) Parse(this IInputLines lines)
    {
        var regex = new PcreRegex("([A-Z]{2}) -> ([A-Z])");

        var paragraphs = lines.AsParagraphs();
        var polymer = new Polymer(new string(paragraphs[0].Span[0].Span));

        var validPairs = new Dictionary<(char, char), char>();
        foreach (var line in paragraphs[1].Span)
        {
            var match = regex.Match(line.Span);
            var span = match.Groups[1].Value;
            var newCharacter = match.Groups[2].Value[0];

            validPairs[(span[0], span[1])] = newCharacter;
        }

        var dictionary = ImmutableDictionary.CreateBuilder<(char, char), Insertion>();
        foreach (var (pair, character) in validPairs)
        {
            var insertions = new List<(char, char)>
            {
                (pair.Item1, character),
                (character, pair.Item2)
            };

            // Remove any new additions that don't expand
            insertions.RemoveAll(pair => validPairs.ContainsKey(pair) is false);

            dictionary.Add(pair, new Insertion(insertions.ToArray(), character));
        }

        return (polymer, dictionary.ToImmutable());
    }
}

readonly record struct Insertion((char, char)[] Insertions, char NewCharacter);

readonly record struct Polymer
{
    public Polymer(string template)
    {
        var span = template.AsSpan();
        var pairs = ImmutableDictionary.CreateBuilder<(char, char), long>();
        var characterCounts = ImmutableDictionary.CreateBuilder<char, long>();

        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];
            var count = characterCounts.GetValueOrDefault(c);
            characterCounts[c] = count + 1;
        }

        for (var i = 0; i < span.Length - 1; i++)
        {
            var pair = (span[i], span[i + 1]);
            var current = pairs.GetValueOrDefault(pair);
            pairs[pair] = current + 1;
        }

        Pairs = pairs.ToImmutable();
        CharacterCounts = characterCounts.ToImmutable();
    }

    public ImmutableDictionary<char, long> CharacterCounts { get; init; }

    public ImmutableDictionary<(char, char), long> Pairs { get; init; }
}
