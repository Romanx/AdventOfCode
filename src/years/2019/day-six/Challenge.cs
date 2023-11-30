using System.Text;
using Shared.Graph;

namespace DaySix2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 6), "Universal Orbit Map");

    public void PartOne(IInput input, IOutput output)
    {
        var graph = input.ParseGraph();

        var total = graph.Vertexes
            .Sum(node => GetOrbitCount(node, graph));

        output.WriteProperty("Total Orbits", total);

        static int GetOrbitCount(string node, Graph<string> nodes)
        {
            if (node == "COM")
                return 0;

            var count = 1;
            foreach (var neighbour in nodes.Neighbours(node))
            {
                count += GetOrbitCount(neighbour, nodes);
            }

            return count;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var graph = input.ParseGraph();
        var myPath = graph.BreadthFirstSearch("YOU", "COM");
        var santaPath = graph.BreadthFirstSearch("SAN", "COM");

        // Find where our paths to COM first collide
        var intersect = myPath.Intersect(santaPath).First();

        // Find how many steps from us until we meet
        var i = myPath.IndexOf(intersect);
        var j = santaPath.IndexOf(intersect);

        // Adding those together make the number of transfers
        // '- 2' for our initial locations
        var result = (i + j) - 2;

        output.WriteProperty("Number of transfers", result);
    }
}

internal static class ParseExtensions
{
    public static Graph<string> ParseGraph(this IInput input)
    {
        var dict = new Dictionary<string, List<string>>();
         foreach (var line in input.Lines.AsMemory())
        {
            var index = line.Span.IndexOf(')');
            var first = new string(line.Span[..index]);
            var second = new string(line.Span[(index + 1)..]);

            dict.AddOrUpdate(
                second,
                static (_, state) => new List<string>() { state },
                static (_, value, state) => { value.Add(state); return value; },
                first);
        }

        var immutable = dict
            .ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableArray());

        return new Graph<string>(immutable);
    }
}
