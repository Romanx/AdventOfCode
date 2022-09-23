using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared.Graph
{
    public static class Algorithms
    {
        /// <remarks>
        /// This uses a faster algorithm for BFS using lists rather than a queue found here:
        /// https://www.redblobgames.com/pathfinding/a-star/implementation.html#optimize-bfs-queue
        /// </remarks>
        public static ImmutableArray<TNode> BreadthFirstSearch<TNode>(
            this IGraph<TNode> graph,
            TNode start,
            TNode goal) where TNode : notnull, IEquatable<TNode>
        {
            var currentFrontier = new List<TNode>();
            var nextFrontier = new List<TNode>();
            currentFrontier.Add(start);
            var cameFrom = new Dictionary<TNode, TNode>
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

            return ReconstructPath(start, goal, cameFrom);
        }

        public static ImmutableArray<TNode> UniformCostSearch<TNode>(
            this IWeightedGraph<TNode> graph,
            TNode start,
            TNode goal) where TNode : notnull, IEquatable<TNode>
        {
            var frontier = new PriorityQueue<TNode, float>();
            frontier.Enqueue(start, 0);

            var cameFrom = new Dictionary<TNode, TNode>()
            {
                [start] = start
            };

            var costSoFar = new Dictionary<TNode, float>()
            {
                [start] = 0
            };

            while (frontier.TryDequeue(out var current, out _))
            {
                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in graph.Neigbours(current))
                {
                    var newCost = costSoFar[current] + graph.Cost(current, next);
                    if (costSoFar.TryGetValue(next, out var nextCost) is false || newCost < nextCost)
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost;
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return ReconstructPath(start, goal, cameFrom);
        }

        public static ImmutableArray<TNode> AStarSearch<TNode>(
            this IWeightedGraph<TNode> graph,
            TNode start,
            TNode goal,
            Func<TNode, TNode, float> heuristicFunction)
            where TNode : notnull, IEquatable<TNode>
        {
            var frontier = new PriorityQueue<TNode, float>();
            frontier.Enqueue(start, 0);

            var cameFrom = new Dictionary<TNode, TNode>()
            {
                [start] = start
            };

            var costSoFar = new Dictionary<TNode, float>()
            {
                [start] = 0
            };

            while (frontier.TryDequeue(out var current, out _))
            {
                if (current.Equals(goal))
                {
                    break;
                }

                foreach (var next in graph.Neigbours(current))
                {
                    var newCost = costSoFar[current] + graph.Cost(current, next);
                    if (costSoFar.TryGetValue(next, out var nextCost) is false || newCost < nextCost)
                    {
                        costSoFar[next] = newCost;
                        var priority = newCost + heuristicFunction(goal, next);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            return ReconstructPath(start, goal, cameFrom);
        }

        public static ImmutableHashSet<TNode> FloodFill<TNode>(
            this IGraph<TNode> graph,
            TNode start,
            uint? distance = null) where TNode : notnull, IEquatable<TNode>
        {
            return FloodFillWithSteps(graph, start, distance)
                .Keys
                .ToImmutableHashSet();
        }

        public static ImmutableDictionary<TNode, int> FloodFillWithSteps<TNode>(
            this IGraph<TNode> graph,
            TNode start,
            uint? distance = null) where TNode : notnull, IEquatable<TNode>
        {
            var visited = ImmutableDictionary.CreateBuilder<TNode, int>();

            var currentFrontier = new List<(TNode, int Steps)>();
            var nextFrontier = new List<(TNode, int Steps)>();
            currentFrontier.Add((start, 0));

            while (currentFrontier.Count > 0)
            {
                foreach (var (current, steps) in currentFrontier)
                {
                    if (distance.HasValue && steps > distance)
                        continue;

                    visited.Add(current, steps);

                    foreach (var next in graph.Neigbours(current))
                    {
                        if (visited.ContainsKey(next) is false)
                        {
                            nextFrontier.Add((next, steps + 1));
                        }
                    }
                }

                (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
                nextFrontier.Clear();
            }

            return visited.ToImmutable();
        }

        public static ImmutableHashSet<TNode> FindConnected<TNode>(
            this IGraph<TNode> graph,
            TNode source) where TNode : notnull, IEquatable<TNode>
        {
            var currentFrontier = new List<TNode>();
            var nextFrontier = new List<TNode>();
            currentFrontier.Add(source);
            var visited = ImmutableHashSet.CreateBuilder<TNode>();
            visited.Add(source);

            while (currentFrontier.Count > 0)
            {
                foreach (var current in currentFrontier)
                {
                    foreach (var next in graph.Neigbours(current))
                    {
                        if (visited.Add(next) is true)
                        {
                            nextFrontier.Add(next);
                        }
                    }
                }

                (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
                nextFrontier.Clear();
            }

            return visited.ToImmutable();
        }

        public static ImmutableArray<TNode> ReconstructPath<TNode>(
            TNode start,
            TNode goal,
            Dictionary<TNode, TNode> cameFrom)
            where TNode : notnull, IEquatable<TNode>
        {
            var current = goal;
            var path = ImmutableArray.CreateBuilder<TNode>();
            while (!current.Equals(start))
            {
                path.Add(current);

                if (cameFrom.TryGetValue(current, out current) is false)
                {
                    return ImmutableArray<TNode>.Empty;
                }
            }
            path.Add(start);
            path.Reverse();
            return path.ToImmutable();
        }

        public static ImmutableArray<T> TopologicalSort<T>(
            IVertexGraph<T> graph,
            Func<T, int>? costFunction = null)
            where T : IEquatable<T>
        {
            var indegree = new Dictionary<T, int>(graph.Vertexes.Length);
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

            var noIncomingEdges = new PriorityQueue<T, int>();
            foreach (var node in graph.Vertexes)
            {
                if (indegree[node] == 0)
                {
                    var cost = costFunction?.Invoke(node) ?? 0;
                    noIncomingEdges.Enqueue(node, cost);
                }
            }

            var ordered = ImmutableArray.CreateBuilder<T>(graph.Vertexes.Length);

            while (noIncomingEdges.TryDequeue(out var node, out _))
            {
                ordered.Add(node);

                foreach (var neighbour in graph.Neigbours(node))
                {
                    indegree[neighbour]--;

                    if (indegree[neighbour] == 0)
                    {
                        var cost = costFunction?.Invoke(neighbour) ?? 0;
                        noIncomingEdges.Enqueue(neighbour, cost);
                    }
                }
            }

            if (ordered.Count == graph.Vertexes.Length)
            {
                return ordered.MoveToImmutable();
            }

            throw new InvalidOperationException("Graph has a cycle! No topological ordering exists.");
        }
    }
}
