using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;

namespace Shared.Graph
{
    /// <summary>
    /// Used to find Cliques in a vertex graph
    /// https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BronKerbosch<T>
        where T : notnull, IEquatable<T>
    {
        private readonly IVertexGraph<T> _graph;

        public BronKerbosch(IVertexGraph<T> graph)
        {
            _graph = graph;
        }

        public IEnumerable<ISet<T>> AllCliques()
        {
            return FindCliques(
                _graph,
                potential_clique: _graph.Vertexes.ToImmutableHashSet(),
                remaining_nodes: ImmutableHashSet<T>.Empty,
                skip_nodes: ImmutableHashSet<T>.Empty);
        }

        public ISet<T> FindMaximalClique()
        {
            ISet<T> maximalClique = ImmutableHashSet<T>.Empty;
            foreach (var clique in AllCliques())
            {
                if (clique.Count > maximalClique.Count)
                {
                    maximalClique = clique;
                }
            }

            return maximalClique;
        }

        private static IEnumerable<ImmutableHashSet<T>> FindCliques(
            IGraph<T> graph,
            ImmutableHashSet<T> potential_clique,
            ImmutableHashSet<T> remaining_nodes,
            ImmutableHashSet<T> skip_nodes)
        {
            if (potential_clique.IsEmpty && skip_nodes.IsEmpty)
            {
                yield return remaining_nodes;
            }
            else
            {
                var choices = potential_clique.Union(skip_nodes);
                ImmutableHashSet<T> pivoted;

                if (choices.Count > 0)
                {
                    var pivotVertex = choices.Shuffle().First();
                    pivoted = potential_clique.Except(graph.Neighbours(pivotVertex));
                }
                else
                {
                    pivoted = potential_clique;
                }

                foreach (var v in pivoted)
                {
                    var neighborsOfV = graph.Neighbours(v);
                    var sub = FindCliques(
                        graph,
                        potential_clique.Intersect(neighborsOfV),
                        remaining_nodes.Add(v),
                        skip_nodes.Intersect(neighborsOfV));

                    foreach (var s in sub)
                        yield return s;

                    potential_clique = potential_clique.Remove(v);
                    skip_nodes = skip_nodes.Add(v);
                }
            }
        }
    }
}
