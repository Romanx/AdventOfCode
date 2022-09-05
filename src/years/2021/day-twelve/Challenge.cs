using System.Runtime.InteropServices;
using Microsoft.Collections.Extensions;
using CommunityToolkit.HighPerformance;

namespace DayTwelve2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 12), "Passage Pathing");

    public void PartOne(IInput input, IOutput output)
    {
        var graph = input.Lines.ParseGraph();
        var paths = FindPaths(
            graph,
            CaveFunction);

        output.WriteProperty("Number of paths", paths);

        static bool CaveFunction(Cave cave, CaveRoute route)
        {
            return route.SmallCaveVisits
                .ContainsKey(cave) is false;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var graph = input.Lines.ParseGraph();
        var paths = FindPaths(graph, CaveFunction);

        output.WriteProperty("Number of paths", paths);

        static bool CaveFunction(Cave cave, CaveRoute route)
        {
            var visitedTwice = route.SmallCaveVisits.Any(kvp => kvp.Value is 2);

            // If i've not visited any twice it doesn't matter just visit it.
            if (visitedTwice is false)
            {
                return true;
            }

            return route.SmallCaveVisits.ContainsKey(cave) is false;
        }
    }

    static int FindPaths(
        ImmutableDictionary<string, Cave> graph,
        Func<Cave, CaveRoute, bool> caveFunc)
    {
        var pathCount = 0;

        var (_, start) = graph.First(g => g.Value.CaveType is CaveType.Entrance);
        var currentFrontier = new List<CaveRoute>();
        var nextFrontier = new List<CaveRoute>();
        currentFrontier.Add(new CaveRoute(ImmutableArray.Create(start), ImmutableDictionary<Cave, int>.Empty));

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                var currentCave = current.Path[^1];

                foreach (var next in currentCave.Connections)
                {
                    var nextCave = graph[next];

                    // If we're at the target then yay
                    if (nextCave.CaveType is CaveType.Exit)
                    {
                        pathCount++;
                    }
                    // If it's the start we can't go back
                    else if (nextCave.CaveType is CaveType.Entrance)
                    {
                    }
                    // It's a small cave!
                    else if (nextCave.CaveType is CaveType.Small)
                    {
                        if (caveFunc(nextCave, current))
                        {
                            var count = current.SmallCaveVisits.GetValueOrDefault(nextCave);

                            nextFrontier.Add(current with
                            {
                                Path = current.Path.Add(nextCave),
                                SmallCaveVisits = current.SmallCaveVisits.SetItem(nextCave, count + 1)
                            });
                        }
                    }
                    else
                    {
                        nextFrontier.Add(current with
                        {
                            Path = current.Path.Add(nextCave)
                        });
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }

        return pathCount;
    }
}

internal static class ParseExtensions
{
    public static ImmutableDictionary<string, Cave> ParseGraph(this IInputLines lines)
    {
        var map = new Dictionary<string, List<string>>();
        foreach (var line in lines.AsString())
        {
            var split = line.Split('-');
            var start = split[0];
            var end = split[1];

            map.TryAdd(start, new List<string>());
            map.TryAdd(end, new List<string>());

            map[start].Add(end);
            map[end].Add(start);
        }

        return map
            .ToImmutableDictionary(k => k.Key, kvp =>
            {
                var id = kvp.Key;
                var caveType = id switch
                {
                    "start" => CaveType.Entrance,
                    "end" => CaveType.Exit,
                    // This is a little hack but based on our input data if the first char is lower then the rest is too
                    _ when char.IsLower(id[0]) => CaveType.Small,
                    _ => CaveType.Large,
                };

                return new Cave(id, caveType, kvp.Value.ToImmutableArray());
            });
    }
}

readonly record struct Cave(string Id, CaveType CaveType, ImmutableArray<string> Connections);

enum CaveType
{
    Entrance,
    Exit,
    Small,
    Large,
}

record CaveRoute(
    ImmutableArray<Cave> Path,
    ImmutableDictionary<Cave, int> SmallCaveVisits);
