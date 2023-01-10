using CommunityToolkit.HighPerformance;

namespace DaySixteen2022;

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"Valve (?<Valve>[A-Z]+) has flow rate=(?<FlowRate>\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (?<Valves>.*)");

    public static ImmutableDictionary<Valve, ImmutableArray<Valve>> Parse(this IInput input)
    {
        var valveMap = new Dictionary<string, (Valve, List<string>)>();

        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);
            var name = new string(match.Groups["Valve"].Value);
            var flowRate = byte.Parse(match.Groups["FlowRate"].Value);

            var links = new List<string>();
            foreach (var linkedValve in match.Groups["Valves"].Value.Tokenize(','))
            {
                links.Add(new string(linkedValve.Trim()));
            }

            var valve = new Valve(name, flowRate);
            valveMap[name] = (valve, links);
        }

        var builder = ImmutableDictionary.CreateBuilder<Valve, ImmutableArray<Valve>>();

        foreach (var (valve, links) in valveMap.Values)
        {
            builder[valve] = links
                .Select(l => valveMap[l].Item1)
                .ToImmutableArray();
        }

        return builder.ToImmutable();
    }
}
