using CommunityToolkit.HighPerformance;

namespace DaySixteen2022;

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"Valve (?<Valve>[A-Z]+) has flow rate=(?<FlowRate>\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (?<Valves>.*)");

    public static ValveGraph Parse(this IInput input)
    {
        var valveMap = new Dictionary<Valve, List<string>>();

        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);
            var name = new string(match.Groups["Valve"].Value);
            var flowRate = byte.Parse(match.Groups["FlowRate"].Value);

            var linked = new List<string>();
            foreach (var linkedValve in match.Groups["Valves"].Value.Tokenize(','))
            {
                linked.Add(new string(linkedValve.Trim()));
            }

            var valve = new Valve(name, flowRate);
            valveMap[valve] = linked;
        }

        var dict = ImmutableDictionary.CreateBuilder<Valve, ImmutableArray<Valve>>();
        dict.AddRange(valveMap.Keys.Select(v => KeyValuePair.Create(v, ImmutableArray<Valve>.Empty)));

        foreach (var (valve, linked) in valveMap)
        {
            dict[valve] = linked
                .Select(l => dict.Keys.First(k => k.Name == l))
                .ToImmutableArray();
        }

        return new ValveGraph(dict.ToImmutable());
    }
}
