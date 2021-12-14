namespace DayTwelve2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 12), "Passage Pathing");

    public override void PartOne(IInput input, IOutput output)
    {
        var graph = input.Lines.ParseGraph();
        var paths = FindPaths(
            graph,
            "start",
            "end",
            CaveFunction);

        output.WriteProperty("Number of paths", paths);

        static bool CaveFunction(char cave, CaveRoute route)
        {
            return route.SmallCaveVisits.ContainsKey(cave) is false;
        }
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var graph = input.Lines.ParseGraph();
        var paths = FindPaths(graph, "start", "end", CaveFunction);

        output.WriteProperty("Number of paths", paths);

        static bool CaveFunction(char cave, CaveRoute route)
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

    int FindPaths(
        Graph graph,
        string start,
        string target,
        Func<char, CaveRoute, bool> caveFunc)
    {
        var pathCount = 0;

        var currentFrontier = new List<CaveRoute>();
        var nextFrontier = new List<CaveRoute>();
        currentFrontier.Add(new CaveRoute(ImmutableArray.Create(start), ImmutableDictionary<char, int>.Empty));

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                foreach (var next in graph.GetEdges(current.Path[^1]))
                {
                    // If we're at the target then yay
                    if (next == target)
                    {
                        pathCount++;
                    }
                    // If it's the start we can't go back
                    else if (next == start)
                    {
                    }
                    // It's a small cave!
                    else if (char.IsLower(next[0]))
                    {
                        var cave = next[0];
                        if (caveFunc(cave, current))
                        {
                            var count = current.SmallCaveVisits.GetValueOrDefault(cave);

                            nextFrontier.Add(current with
                            {
                                Path = current.Path.Add(next),
                                SmallCaveVisits = current.SmallCaveVisits.SetItem(cave, count + 1)
                            });
                        }
                    }
                    else
                    {
                        nextFrontier.Add(current with
                        {
                            Path = current.Path.Add(next)
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
    public static Graph ParseGraph(this IInputLines lines)
    {
        var links = new Dictionary<string, List<string>>();
        foreach (var line in lines.AsString())
        {
            var split = line.Split('-', StringSplitOptions.TrimEntries);

            AddEdge(links, split[0], split[1]);
        }

        return new Graph(links);

        static void AddEdge(Dictionary<string, List<string>> links, string to, string from)
        {
            if (links.TryGetValue(to, out var toEdges) is false)
            {
                toEdges = links[to] = new List<string>();
            }

            if (links.TryGetValue(from, out var fromEdges) is false)
            {
                fromEdges = links[from] = new List<string>();
            }

            toEdges.Add(from);
            fromEdges.Add(to);
        }
    }
}

class Graph
{
    private readonly ImmutableDictionary<string, ImmutableArray<string>> _links;

    public Graph(IDictionary<string, List<string>> links)
    {
        var nodes = ImmutableHashSet.CreateBuilder<string>();
        nodes.UnionWith(links.Keys);
        nodes.UnionWith(links.Values.SelectMany(v => v));

        Nodes = nodes.ToImmutable();
        _links = links.ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableArray());
    }

    public ImmutableHashSet<string> Nodes { get; }

    public IEnumerable<string> GetEdges(string node)
        => _links.TryGetValue(node, out var edges) ? edges : Enumerable.Empty<string>();
}

record CaveRoute(
    ImmutableArray<string> Path,
    ImmutableDictionary<char, int> SmallCaveVisits);
