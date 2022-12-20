using Shared.Graph;

namespace DaySixteen2022;

class SearchGraph : IGraph<SearchState>
{
    private readonly ImmutableDictionary<Valve, ImmutableHashSet<(Valve Valve, ushort Distance)>> valveMap;

    public uint MaxTime { get; }

    public SearchGraph(
        ImmutableDictionary<Valve, ImmutableHashSet<(Valve Valve, ushort Distance)>> valveMap,
        uint maxTime)
    {
        this.valveMap = valveMap;
        MaxTime = maxTime;
    }

    public IEnumerable<SearchState> Neigbours(SearchState node)
    {
        var time = node.CurrentTime;

        if (time > MaxTime)
        {
            yield break;
        }

        foreach (var current in node.CurrentPositions)
        {
            if (current.FlowRate > 0 &&
                node.OpenValves.Contains(current.Name) is false &&
                (time + 1) <= MaxTime)
            {
                yield return node.OpenValve(current, time + 1, MaxTime);
            }
        }

        if (currentValve.FlowRate > 0 &&
            openValves.Contains(currentValve.Name) is false &&
            (time + 1) <= MaxTime)
        {
            yield return node.OpenValve(currentValve, time + 1, MaxTime);
        }

        var connected = valveMap[currentValve];
        foreach (var (next, distance) in connected)
        {
            yield return node with
            {
                CurrentTime = time + distance,
                CurrentValve = next,
                Path = node.Path.Add(next.Name),
            };
        }
    }
}
