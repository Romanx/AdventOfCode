namespace DayEight2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 8), "Seven Segment Search");

    public void PartOne(IInput input, IOutput output)
    {
        var entries = input.Lines.Parse();
        var count = 0;
        foreach (var entry in entries)
        {
            count += entry.OutputValues
                .Count(static value =>
                {
                    return value.Length switch
                    {
                        2 => true, // Number 1
                        3 => true, // Number 7
                        4 => true, // Number 4
                        7 => true, // Number 8
                        _ => false
                    };
                });
        }

        output.WriteProperty("Digit Count", count);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var entries = input.Lines.Parse();

        var result = entries
            .Sum(entry => DecodeEntry(entry));

        output.WriteProperty("Result", result);
    }

    static int DecodeEntry(Entry entry)
    {
        var decoded = DecodeDisplay(entry.SignalPatterns);
        var result = 0;

        foreach (var item in entry.OutputValues)
        {
            result = (result * 10) + Array.IndexOf(decoded, item);
        }

        return result;
    }

    static string[] DecodeDisplay(ISet<string> signalPatterns)
    {
        var map = new string[10];

        // Easy Numbers
        map[1] = signalPatterns.First(x => x.Length == 2);
        map[4] = signalPatterns.First(x => x.Length == 4);
        map[7] = signalPatterns.First(x => x.Length == 3);
        map[8] = signalPatterns.First(x => x.Length == 7);

        // Hard Numbers
        map[9] = signalPatterns.First(x =>
            x.Length == 6 &&
            map[4].Intersect(x).Count() == 4);

        map[0] = signalPatterns.First(x =>
            x.Length == 6 &&
            x != map[9] &&
            map[1].Intersect(x).Count() == 2);

        map[6] = signalPatterns.First(x =>
            x.Length == 6 &&
            x != map[9] &&
            x != map[0]);

        map[3] = signalPatterns.First(x =>
            x.Length == 5 &&
            map[1].Intersect(x).Count() == 2);

        map[5] = signalPatterns.First(x =>
            x.Length == 5 &&
            x != map[3] &&
            map[9].Intersect(x).Count() == 5);

        map[2] = signalPatterns.First(x =>
            x.Length == 5 &&
            x != map[3] &&
            x != map[5]);

        return map;
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Entry> Parse(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<Entry>();

        foreach (var line in lines.AsString())
        {
            var split = line.Split('|', StringSplitOptions.TrimEntries);
            var patterns = split[0]
                .Split(' ', StringSplitOptions.TrimEntries)
                .Select(x => SortStringOrder(x))
                .ToImmutableHashSet();

            var outputValues = split[1]
                .Split(' ', StringSplitOptions.TrimEntries)
                .Select(x => SortStringOrder(x))
                .ToImmutableArray();

            builder.Add(new Entry(patterns, outputValues));
        }

        return builder.ToImmutable();

        static string SortStringOrder(string input)
        {
            var characters = input.ToCharArray().AsSpan();
            characters.Sort();
            return new string(characters);
        }
    }
}

record Entry(ImmutableHashSet<string> SignalPatterns, ImmutableArray<string> OutputValues);
