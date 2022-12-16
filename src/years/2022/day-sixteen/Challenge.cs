using CommunityToolkit.HighPerformance;

namespace DaySixteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 16), "Proboscidea Volcanium");

    public void PartOne(IInput input, IOutput output)
    {
        var parsed = input.Parse();

        RunGraph(parsed);
    }

    public void PartTwo(IInput input, IOutput output)
    {
    }

    public object RunGraph(ImmutableDictionary<Valve, ImmutableArray<Valve>> parsed)
    {
        var currentFrontier = new List<SearchState>();
        var nextFrontier = new List<SearchState>();
        var start = SearchState.Empty;

        currentFrontier.Add(start);
        var cameFrom = new Dictionary<SearchState, SearchState>
        {
            [start] = start,
        };

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                var linked = parsed[current.CurrentValve];

                foreach (var next in linked)
                {
                    var nextState = current with
                    {
                        CurrentValve = next,
                        CurrentTime = current.CurrentTime + 1,
                    };

                    if (cameFrom.ContainsKey(nextState) is false)
                    {
                        nextFrontier.Add(nextState);
                        cameFrom[nextState] = current;
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"Valve (?<Valve>[A-Z]+) has flow rate=(?<FlowRate>\d+); tunnel(?:s)? lead(?:s)? to valve(?:s)? (?<Valves>.*)");

    public static ImmutableDictionary<Valve, ImmutableArray<Valve>> Parse(this IInput input)
    {
        var valveMap = new Dictionary<Valve, List<string>>();

        foreach (var line in input.Lines.AsMemory())
        {
            var match = regex.Match(line.Span);
            var name = new string(match.Groups["Valve"].Value);
            var flowRate = uint.Parse(match.Groups["FlowRate"].Value);

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

        return dict.ToImmutable();
    }
}

readonly record struct Valve(string Name, uint FlowRate);

readonly record struct SearchState(
    Valve CurrentValve,
    uint CurrentTime,
    ImmutableArray<(string Valve, uint TimeOpened)> Valves) : IEquatable<SearchState>
{
    public static SearchState Empty { get; } = new SearchState(
        new Valve("AA", 0),
        0,
        ImmutableArray<(string Valve, uint TimeOpened)>.Empty);

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(CurrentValve);
        hash.Add(CurrentTime);
        foreach (var value in Valves)
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    public bool Equals(SearchState other)
    {
        return other.CurrentTime == CurrentTime
            && other.CurrentValve == CurrentValve
            && Valves.SequenceEqual(other.Valves);
    }
}
