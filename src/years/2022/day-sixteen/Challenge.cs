using System.Linq;
using Shared.Graph;
using static MoreLinq.Extensions.SubsetsExtension;

namespace DaySixteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 16), "Proboscidea Volcanium");

    public void PartOne(IInput input, IOutput output)
    {
        const uint minutes = 30;
        var valveMap = input.Parse();
        
        var start = new SearchState(
            ImmutableArray.Create(new Valve("AA", 0)),
            0,
            0,
            ImmutableHashSet<string>.Empty);

        var stateMap = RunGraph(start, new SearchGraph(valveMap, minutes));
        var best = stateMap.Values.MaxBy(s => s.TotalPressure);

        output.WriteProperty("Total Pressure", best.TotalPressure);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const uint minutes = 26;
        var valveMap = input.Parse();
        var start = new SearchState(
            ImmutableArray.Create(new Valve("AA", 0), new Valve("AA", 0)),
            0,
            0,
            ImmutableHashSet<string>.Empty);

        var stateMap = RunGraph(start, new SearchGraph(valveMap, minutes));
        var best = stateMap.Values.MaxBy(s => s.TotalPressure);

        output.WriteProperty("Max Pressure", best.TotalPressure);
    }

    ImmutableDictionary<SearchState, SearchState> RunGraph(SearchState start, SearchGraph graph)
    {
        var currentFrontier = new List<SearchState>();
        var nextFrontier = new List<SearchState>();

        currentFrontier.Add(start);
        var cameFrom = new Dictionary<SearchState, SearchState>
        {
            [start] = start,
        };

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                foreach (var next in graph.Neigbours(current))
                {
                    if (cameFrom.ContainsKey(next) is false)
                    {
                        nextFrontier.Add(next);
                        cameFrom[next] = current;
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }

        return cameFrom.ToImmutableDictionary();
    }
}
