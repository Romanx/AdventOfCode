using Shared.Graph;

namespace DaySeven2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 7), "The Sum of Its Parts");

    public void PartOne(IInput input, IOutput output)
    {
        var graph = input.Parse();

        var sorted = Algorithms.TopologicalSort(graph, costFunction: static vertex => vertex);

        output.WriteProperty("Vertexes", string.Join("", graph.Vertexes));
        output.WriteProperty("Sorted Vertexes", string.Join("", sorted));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var graph = input.Parse();

        var (sorted, time) = MultiWorkerTopologicalSort(graph, 5);

        output.WriteProperty("Vertexes", string.Join("", graph.Vertexes));
        output.WriteProperty("Sorted Vertexes", string.Join("", sorted));
        output.WriteProperty("Total time taken", time);
    }


    private static (ImmutableArray<char> Order, int Time) MultiWorkerTopologicalSort(Graph<char> graph, int totalWorkers)
    {
        var indegree = new Dictionary<char, int>(graph.Vertexes.Length);
        foreach (var node in graph.Vertexes)
        {
            indegree.TryAdd(node, 0);

            foreach (var neighbour in graph.Neigbours(node))
            {
                indegree.AddOrUpdate(
                    neighbour,
                    1,
                    (key, value) => value + 1);
            }
        }

        var noIncomingEdges = new PriorityQueue<char, int>();
        foreach (var node in graph.Vertexes)
        {
            if (indegree[node] == 0)
            {
                var cost = CostFunction(node);
                noIncomingEdges.Enqueue(node, cost);
            }
        }

        var ordered = ImmutableArray.CreateBuilder<char>(graph.Vertexes.Length);

        var workers = new PrioritySet<(char Node, int Start), int>(totalWorkers);
        var second = 0;

        BusyWorkers(totalWorkers, 0, workers, noIncomingEdges);

        while (workers.TryDequeue(out var item, out var time))
        {
            var (node, start) = item;
            second += time;
            ordered.Add(node);

            foreach (var neighbour in graph.Neigbours(node))
            {
                indegree[neighbour]--;

                if (indegree[neighbour] == 0)
                {
                    var cost = CostFunction(node);
                    noIncomingEdges.Enqueue(neighbour, cost);
                }
            }

            foreach (var (element, _) in workers.ToArray())
            {
                var newPriority = NodeTime(element.Node) - (second - element.Start);
                workers.TryUpdate(element, newPriority);
            }

            BusyWorkers(totalWorkers, second, workers, noIncomingEdges);
        }

        return (ordered.MoveToImmutable(), second);

        static void BusyWorkers(
            int totalWorkers,
            int currentTime,
            PrioritySet<(char Node, int Start), int> workers,
            PriorityQueue<char, int> edges)
        {
            while (workers.Count < totalWorkers && edges.TryDequeue(out var node, out _))
            {
                workers.Enqueue((node, currentTime), NodeTime(node));
            }
        }

        static int CostFunction(char task) => task;

        static int NodeTime(char node) => 60 + (node - 'A' + 1);
    }
}
