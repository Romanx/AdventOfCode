using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance;

namespace Shared.Graph
{
    /// <summary>
    /// An algorithm for finding shortest paths in a directed weighted graph with positive or negative edge weights (but with no negative cycles)
    /// https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm
    /// </summary>
    public static class FloydWarshall
    {
        /// <summary>
        /// An algorithm for finding shortest paths in a directed weighted graph with positive or negative edge weights (but with no negative cycles)
        /// https://en.wikipedia.org/wiki/Floyd%E2%80%93Warshall_algorithm
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <returns></returns>
        public static DistanceMap<TNode, TWeightValue> Build<TGraph, TNode, TWeightValue>(TGraph graph)
            where TNode : notnull, IEquatable<TNode>
            where TGraph : IWeightedGraph<TNode, TWeightValue>, IVertexGraph<TNode>
            where TWeightValue : INumber<TWeightValue>, IMinMaxValue<TWeightValue>
        {
            var distances = new TWeightValue[graph.Vertexes.Length, graph.Vertexes.Length];
            distances.AsSpan2D().Fill(TWeightValue.MaxValue);

            for (var i = 0; i < graph.Vertexes.Length; i++)
            {
                var vertex = graph.Vertexes[i];
                foreach (var neighbour in graph.Neighbours(vertex))
                {
                    var index = graph.Vertexes.IndexOf(neighbour);
                    distances[i, index] = graph.Cost(vertex, neighbour);
                }
            }

            for (var i = 0; i < graph.Vertexes.Length; i++)
            {
                distances[i, i] = TWeightValue.Zero;
            }

            for (var k = 0; k < graph.Vertexes.Length; k++)
            {
                for (var i = 0; i < graph.Vertexes.Length; i++)
                {
                    for (var j = 0; j < graph.Vertexes.Length; j++)
                    {
                        var (overflow, result) = OverflowingAdd(distances[i, k], distances[k, j]);

                        if (overflow is false && distances[i, j] > result)
                        {
                            distances[i, j] = result;
                        }
                    }
                }
            }

            var distanceMap = new Dictionary<(TNode, TNode), TWeightValue>(graph.Vertexes.Length);
            for (var i = 0; i < graph.Vertexes.Length; i++)
            {
                var source = graph.Vertexes[i];
                for (var j = 0; j < graph.Vertexes.Length; j++)
                {
                    var destination = graph.Vertexes[j];

                    distanceMap[(source, destination)] = distances[i, j];
                }
            }

            return new DistanceMap<TNode, TWeightValue>(distanceMap);

            static (bool, TWeightValue vale) OverflowingAdd(TWeightValue a, TWeightValue b)
            {
                var overflow = TWeightValue.MaxValue - a < b;

                return overflow
                    ? (overflow, TWeightValue.MaxValue)
                    : (overflow, a + b);
            }
        }
    }

    public record DistanceMap<TNode, TWeight> : IEnumerable<KeyValuePair<(TNode Source, TNode Destination), TWeight>>
    {
        private readonly Dictionary<(TNode Source, TNode Destination), TWeight> map;

        public DistanceMap(Dictionary<(TNode Source, TNode Destination), TWeight> map)
        {
            this.map = map;
        }

        public TWeight this[TNode source, TNode destination] => map[(source, destination)];

        public IEnumerator<KeyValuePair<(TNode Source, TNode Destination), TWeight>> GetEnumerator()
            => map.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => map.GetEnumerator();
    }
}
