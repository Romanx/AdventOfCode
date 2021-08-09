using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Priority_Queue;

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
                        nextFrontier.Add(next);
                        cameFrom[next] = current;
                    }
                }

                currentFrontier = nextFrontier;
                nextFrontier = currentFrontier;
                nextFrontier.Clear();
            }

            return ReconstructPath(start, goal, cameFrom);
        }

        public static ImmutableArray<TNode> UniformCostSearch<TNode>(
            this IWeightedGraph<TNode> graph,
            TNode start,
            TNode goal) where TNode : notnull, IEquatable<TNode>
        {
            var frontier = new SimplePriorityQueue<TNode>();
            frontier.Enqueue(start, 0);

            var cameFrom = new Dictionary<TNode, TNode>()
            {
                [start] = start
            };

            var costSoFar = new Dictionary<TNode, float>()
            {
                [start] = 0
            };

            while (frontier.TryDequeue(out var current))
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
            var frontier = new SimplePriorityQueue<TNode>();
            frontier.Enqueue(start, 0);

            var cameFrom = new Dictionary<TNode, TNode>()
            {
                [start] = start
            };

            var costSoFar = new Dictionary<TNode, float>()
            {
                [start] = 0
            };

            while (frontier.TryDequeue(out var current))
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

        private static ImmutableArray<TNode> ReconstructPath<TNode>(TNode start, TNode goal, Dictionary<TNode, TNode>? cameFrom) where TNode : notnull, IEquatable<TNode>
        {
            var current = goal;
            var path = ImmutableArray.CreateBuilder<TNode>();
            while (!current.Equals(start))
            {
                path.Add(current);
                current = cameFrom[current];
            }
            path.Add(start);
            path.Reverse();
            return path.ToImmutable();
        }
    }
}
