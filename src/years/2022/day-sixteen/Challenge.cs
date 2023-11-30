using System.Collections.Concurrent;
using Shared.Graph;
using static MoreLinq.Extensions.SubsetsExtension;

namespace DaySixteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 16), "Proboscidea Volcanium");

    public void PartOne(IInput input, IOutput output)
    {
        const byte minutes = 30;
        var valves = input.Parse();

        var importantValves = valves
            .Vertexes
            .Where(v => v.FlowRate > 0 || v.Name is "AA")
            .OrderBy(v => v.Name)
            .ToImmutableArray();

        var start = new Valve("AA", 0);
        var distances = FloydWarshall.Build<ValveGraph, Valve, byte>(valves);
        var stateCache = new Dictionary<State, State>();

        var best = RunGraph(stateCache, State.New(start), minutes, importantValves, distances);

        output.WriteProperty("Total Pressure", best.TotalPressure);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const byte minutes = 26;
        var valves = input.Parse();

        var start = new Valve("AA", 0);
        var distances = FloydWarshall.Build<ValveGraph, Valve, byte>(valves);

        var importantValves = valves
            .Vertexes
            .Where(v => v.FlowRate > 0 || v.Name is "AA")
            .OrderBy(v => v.Name)
            .ToImmutableArray();

        // Split the important valves into two groups
        var subsets = importantValves.Subsets(importantValves.Length / 2);

        var stateCache = new ConcurrentDictionary<State, State>();

        var bestPressure = subsets
            .AsParallel()
            .Max(subset =>
            {
                var elfValves = CreateVisistedSet(subset, importantValves);
                var elephantValves = CreateVisistedSet(importantValves.Except(subset), importantValves);

                var elfEnd = RunGraph(stateCache, State.New(start, elfValves), minutes, importantValves, distances);
                var elephantEnd = RunGraph(stateCache, State.New(start, elephantValves), minutes, importantValves, distances);

                return elfEnd.TotalPressure + elephantEnd.TotalPressure;
            });

        var x = ImmutableArray.CreateBuilder<int>();
        x.ToImmutable();

        output.WriteProperty("Total Pressure", bestPressure);

        static ValveSet CreateVisistedSet(IEnumerable<Valve> seen, ImmutableArray<Valve> allValves)
        {
            var valves = ValveSet.CreateBuilder();

            var indexes = seen.Select(v => allValves.IndexOf(v));

            valves.SetRange(indexes);

            return valves.ToValveSet();
        }
    }

    State RunGraph(
        IDictionary<State, State> stateCache,
        State start,
        byte totalMinutes,
        ImmutableArray<Valve> valves,
        DistanceMap<Valve, byte> distances)
    {
        if (stateCache.TryGetValue(start, out var result))
        {
            return result;
        }

        var currentFrontier = new List<State>();
        var nextFrontier = new List<State>();

        currentFrontier.Add(start);

        var bestByVisited = new Dictionary<ValveSet, State>();

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                foreach (var next in current.Next(totalMinutes, valves, distances))
                {
                    if (bestByVisited.TryGetValue(next.Visited, out var currentBest) is false ||
                        next.TotalPressure > currentBest.TotalPressure)
                    {
                        bestByVisited[next.Visited] = next;
                        nextFrontier.Add(next);
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }

        var best = bestByVisited.Values
            .MaxBy(state => state.TotalPressure);
        stateCache[start] = best;

        return best;
    }
}

internal class ValveGraph : IWeightedGraph<Valve, byte>, IVertexGraph<Valve>
{
    private readonly ImmutableDictionary<Valve, ImmutableArray<Valve>> valveMap;

    public ValveGraph(ImmutableDictionary<Valve, ImmutableArray<Valve>> valveMap)
    {
        this.valveMap = valveMap;
        Vertexes = valveMap.Keys.ToImmutableArray();
    }

    public ImmutableArray<Valve> Vertexes { get; }

    public byte Cost(Valve nodeA, Valve nodeB) => 1;

    public IEnumerable<Valve> Neighbours(Valve node)
         => valveMap[node];
}

readonly record struct State(
    Valve CurrentValve,
    ValveSet Visited,
    byte CurrentTime,
    ushort TotalPressure)
{
    public static State New(Valve current)
        => new(current, ValveSet.New(), 0, 0);

    public static State New(Valve current, ValveSet visited)
        => new(current, visited, 0, 0);

    internal IEnumerable<State> Next(
        byte totalMinutes,
        ImmutableArray<Valve> valves,
        DistanceMap<Valve, byte> distances)
    {
        for (var i = 0; i < valves.Length; i++)
        {
            if (Visited.Contains(i))
            {
                continue;
            }

            var candidate = valves[i];

            var distance = distances[CurrentValve, candidate];

            var start = (byte)(CurrentTime + distance);

            if (start > totalMinutes)
            {
                continue;
            }

            var additional = (totalMinutes - (start + 1)) * candidate.FlowRate;

            yield return this with
            {
                CurrentValve = candidate,
                Visited = Visited + i,
                CurrentTime = (byte)(CurrentTime + (distance + 1)),
                TotalPressure = (ushort)(TotalPressure + additional),
            };
        }
    }
}

readonly record struct Valve(string Name, byte FlowRate);
