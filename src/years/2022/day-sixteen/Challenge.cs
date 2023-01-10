using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;
using MoreLinq;
using Shared.Graph;

namespace DaySixteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 16), "Proboscidea Volcanium");

    public void PartOne(IInput input, IOutput output)
    {
        var valveMap = input.Parse();
        var graph = new SearchGraph(valveMap);
        var parsed = BuildParsedInput(graph);

        ushort best = 0;

        BranchAndBound(
            parsed.FlowRateForIndex,
            parsed.OrderedFlowRateIndexes,
            parsed.ShortestPathLengths,
            State.New(parsed.StartingNode, 30),
            new Dictionary<ushort, ushort>(),
            ref best,
            (bound, best) => bound > best);

        output.WriteProperty("Total Pressure Released", best);
    }

    public void PartTwo(IInput input, IOutput output)
    {
    }

    private static void BranchAndBound(
        ImmutableArray<byte> flowRates,
        ImmutableArray<byte> orderedFlowRateIndexes,
        Memory2D<byte> shortestPathLengths,
        State state,
        Dictionary<ushort, ushort> bestForVisited,
        ref ushort best,
        Func<ushort, ushort, bool> filterBound)
    {
        if (bestForVisited.TryGetValue(state.Visited, out var currentBest) is false)
        {
            currentBest = bestForVisited[state.Visited] = ushort.Max(state.PressureReleased, currentBest);
        }

        best = ushort.Max(state.PressureReleased, best);

        var next = new List<(ushort Bound, State Branch)>();
        foreach (var branch in state.Branch(flowRates, shortestPathLengths))
        {
            var bound = branch.Bound(flowRates, orderedFlowRateIndexes);

            if (filterBound(bound, best) is false)
                continue;

            next.Add((bound, branch));
        }

        foreach (var (bound, branch) in next.OrderByDescending(pair => pair.Bound))
        {
            if (filterBound(bound, best)) 
            {
                BranchAndBound(
                    flowRates,
                    orderedFlowRateIndexes,
                    shortestPathLengths,
                    branch,
                    bestForVisited,
                    ref best,
                    filterBound);
            }
        }
    }

    private static ParsedInput BuildParsedInput(SearchGraph graph)
    {
        var distances = Algorithms.FloydWarshall<SearchGraph, Valve, byte>(graph);

        var interestingValves = graph.Vertexes
            .Select((valve, index) => (valve, index))
            .Where(kvp => kvp.valve.Name == "AA" || kvp.valve.FlowRate > 0)
            .Select(kvp => kvp.index)
            .ToArray();

        var flowRates = interestingValves
            .Select(i => graph.Vertexes[i].FlowRate)
            .ToImmutableArray();

        var shortest = new byte[
            interestingValves.Length,
            interestingValves.Length];

        for (var i = 0; i < interestingValves.Length; i++)
        {
            var iIndex = interestingValves[i];
            for (var j = 0; j < interestingValves.Length; j++)
            {
                var jIndex = interestingValves[j];

                shortest[i, j] = distances[iIndex, jIndex];
            }
        }

        var start = interestingValves
            .Where(i => graph.Vertexes[i].Name is "AA")
            .First();

        var sortedFlowRates = flowRates
            .Select((rate, index) => (rate, index))
            .OrderByDescending(kvp => kvp.rate)
            .Select(kvp => (byte)kvp.index)
            .ToImmutableArray();

        return new ParsedInput(flowRates, shortest, sortedFlowRates, (byte)start);
    }
}

readonly record struct ParsedInput(
    ImmutableArray<byte> FlowRateForIndex,
    Memory2D<byte> ShortestPathLengths,
    ImmutableArray<byte> OrderedFlowRateIndexes,
    byte StartingNode);

readonly record struct State(
    ushort Visited,
    ushort Avoid,
    ushort PressureReleased,
    byte MinutesRemaining,
    byte Position)
{
    public static State New(byte position, byte minutesRemaning) => new State(
        0, (ushort)(1 << position), 0, minutesRemaning, position);

    public bool CanVisit(byte i)
    {
        var x = Visited | Avoid;
        var y = 1 << i;

        return (x & y) == 0;
    }

    public ushort Bound(ImmutableArray<byte> flowRates, ImmutableArray<byte> sortedFlowRateIndices)
    {
        var current = this;
        
        // Make the assumption that if all valves were one apart.
        // It'd take on minute to get to the valve and one minute to turn it on. So Take every 2.
        // Take each valve we can visit and add their flow rates and when we could plausibily turn them on.

        var future = (ushort)(0..(MinutesRemaining + 1)).ToEnumerable()
            .Reverse()
            .TakeEvery(2)
            .Skip(1)
            .Zip(sortedFlowRateIndices.Where(current.CanVisit).Select(i => flowRates[i]))
            .Sum(kvp => kvp.First * kvp.Second);

        return (ushort)(PressureReleased + future);
    }

    public IEnumerable<State> Branch(ImmutableArray<byte> flowRates, Memory2D<byte> shortestPathLengths)
    {
        var span = shortestPathLengths.Span;
        var row = span.GetRowSpan(Position);
        var branches = new State[row.Length];

        for (byte i = 0; i < row.Length; i++)
        {
            var destination = i;
            var distance = row[i];

            if (CanVisit(i))
            {
                var minutesRemaining = MinutesRemaining - (distance + 1);

                branches[i] = this with
                {
                    Visited = (ushort)(Visited | (ushort)(1u << destination)),
                    PressureReleased = (ushort)(PressureReleased + (minutesRemaining * flowRates[i])),
                    Position = destination
                };
            }
        }

        return branches;
    }
}
