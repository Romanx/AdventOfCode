using Shared.Graph;

namespace DaySeven2018;

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new("Step ([A-Z]) must be finished before step ([A-Z]) can begin.");

    public static Graph<char> Parse(this IInput input)
    {
        var dict = new Dictionary<char, List<char>>();
        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);
            var start = match.Groups[1].Value[0];
            var end = match.Groups[2].Value[0];

            dict.AddOrUpdate(
                start,
                static (_, state) => new List<char>() { state },
                static (_, value, state) => { value.Add(state); return value; },
                end);
        }

        var immutable = dict
            .ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableArray());

        return new Graph<char>(immutable);
    }
}
