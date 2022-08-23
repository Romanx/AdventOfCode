using Shared.Graph;

namespace DayTwelve2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 12), "Digital Plumber");

        public void PartOne(IInput input, IOutput output)
        {
            var graph = input.Lines.ParseGraph();

            var connected = graph.FindConnected(0);

            output.WriteProperty("Connected Group Size", connected.Count);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var graph = input.Lines.ParseGraph();
            var nodes = graph.Nodes;

            var groups = 0;
            while (nodes.Count > 0)
            {
                var connections = graph.FindConnected(nodes.First());
                nodes = nodes.Except(connections);
                groups++;
            }


            output.WriteProperty("Number of Groups", groups);
        }
    }

    internal static class ParseExtensions
    {
        public static Graph ParseGraph(this IInputLines lines)
        {
            var connections = ParseConnections(lines);
            return new Graph(connections);
        }

        private static readonly PcreRegex regex = new(@"^(?<From>\d+) <-> (?<To>.*)$");

        private static ImmutableDictionary<int, ImmutableHashSet<int>> ParseConnections(IInputLines lines)
        {
            var builder = new Dictionary<int, HashSet<int>>();

            foreach (var line in lines.AsMemory())
            {
                var match = regex.Match(line.Span);

                var source = int.Parse(match.Groups["From"].Value);
                var connections = match.Groups["To"].Value
                    .ToString()
                    .Split(",")
                    .Select(int.Parse)
                    .ToHashSet();

                builder.Add(source, connections);
            }

            return builder
                .ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableHashSet());
        }
    }

    public class Graph : IGraph<int>
    {
        private readonly ImmutableDictionary<int, ImmutableHashSet<int>> _connections;

        public ImmutableHashSet<int> Nodes { get; }

        public Graph(ImmutableDictionary<int, ImmutableHashSet<int>> connections)
        {
            _connections = connections;
            Nodes = _connections.Keys.ToImmutableHashSet();
        }

        public IEnumerable<int> Neigbours(int node) =>
            _connections[node];
    }
}
