namespace DayTen2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 10), "Syntax Scoring");

    private static readonly Dictionary<char, char> bracketMap = new()
    {
        ['('] = ')',
        ['['] = ']',
        ['{'] = '}',
        ['<'] = '>',
    };

    public override void PartOne(IInput input, IOutput output)
    {
        var total = input.Lines.AsMemory()
            .Select(line => ScoreLine(line.Span))
            .Where(line => line.Type is LineType.Corrupted)
            .Sum(line => line.Score);

        output.WriteProperty("Error Score", total);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var scores = input.Lines.AsMemory()
            .Select(line => ScoreLine(line.Span))
            .Where(line => line.Type is LineType.Incomplete)
            .OrderBy(x => x.Score)
            .ToArray();

        var median = scores[scores.Length / 2];

        output.WriteProperty("Autocomplete Score", median.Score);
    }

    static LineResult ScoreLine(ReadOnlySpan<char> line)
    {
        var chunks = new Stack<char>();
        foreach (var c in line)
        {
            // If it's an open chunk
            if (bracketMap.ContainsKey(c))
            {
                chunks.Push(c);
            }
            else
            {
                var current = chunks.Pop();
                var close = bracketMap[current];
                if (c != close)
                {
                    return new LineResult(LineType.Corrupted, ScoreCorrupted(c));
                }
            }
        }

        var autocompleteScore = chunks
            .Select(c => bracketMap[c])
            .Aggregate(0L, (acc, c) => (acc * 5) + ScoreAutocomplete(c));

        return new LineResult(LineType.Incomplete, autocompleteScore);

        static int ScoreCorrupted(char c) => c switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new InvalidOperationException($"Not a valid scoring character: '{c}'")
        };

        static int ScoreAutocomplete(char c) => c switch
        {
            ')' => 1,
            ']' => 2,
            '}' => 3,
            '>' => 4,
            _ => throw new InvalidOperationException($"Not a valid scoring character: '{c}'")
        };
    }
}

readonly record struct LineResult(LineType Type, long Score);

enum LineType
{
    Incomplete,
    Corrupted,
}
