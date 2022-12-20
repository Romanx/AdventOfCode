using CommunityToolkit.HighPerformance;

namespace DaySixteen2022;

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"Valve (?<Valve>[A-Z]+) has flow rate=(?<FlowRate>\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (?<Valves>.*)");

    public static ImmutableDictionary<Valve, ImmutableHashSet<(Valve Valve, ushort Distance)>> Parse(this IInput input)
    {
        var valveMap = new Dictionary<string, (Valve, List<string>)>();

        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);
            var name = new string(match.Groups["Valve"].Value);
            var flowRate = uint.Parse(match.Groups["FlowRate"].Value);

            var links = new List<string>();
            foreach (var linkedValve in match.Groups["Valves"].Value.Tokenize(','))
            {
                links.Add(new string(linkedValve.Trim()));
            }

            var valve = new Valve(name, flowRate);
            valveMap[name] = (valve, links);
        }

        var builder = ImmutableDictionary.CreateBuilder<Valve, ImmutableHashSet<(Valve, ushort)>>();

        foreach (var (valve, links) in valveMap.Values)
        {
            if (valve.FlowRate == 0 && valve.Name is not "AA")
                continue;

            var connections = ImmutableHashSet.CreateBuilder<(Valve, ushort)>();
            var visited = ImmutableHashSet.CreateBuilder<Valve>();
            visited.Add(valve);

            FlattenUselessValves(
                1,
                valveMap,
                connections,
                links,
                visited);

            builder[valve] = connections.ToImmutable();
        }

        return builder.ToImmutable();

        static void FlattenUselessValves(
            ushort distance,
            Dictionary<string, (Valve, List<string>)> valveMap,
            ImmutableHashSet<(Valve, ushort)>.Builder connections,
            IEnumerable<string> links,
            ImmutableHashSet<Valve>.Builder visited)
        {
            foreach (var link in links)
            {
                var (otherValve, otherLinks) = valveMap[link];

                if (visited.Contains(otherValve))
                    continue;

                visited.Add(otherValve);

                if (otherValve.FlowRate > 0)
                {
                    connections.Add((otherValve, distance));
                }
                else
                {
                    FlattenUselessValves((ushort)(distance + 1), valveMap, connections, otherLinks, visited);
                }
            }
        }
    }
}
