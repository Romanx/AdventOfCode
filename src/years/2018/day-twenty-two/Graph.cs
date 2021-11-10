using System.Diagnostics;
using Shared.Graph;

namespace DayTwentyTwo2018
{
    internal class Graph : IWeightedGraph<Node>
    {
        private static readonly ImmutableArray<Tool> Tools = ImmutableArray.Create(Tool.ClimbingGear, Tool.Torch, Tool.None);
        private static readonly ImmutableArray<Tool> RockyRegionTools = ImmutableArray.Create(Tool.ClimbingGear, Tool.Torch);
        private static readonly ImmutableArray<Tool> WetRegionTools = ImmutableArray.Create(Tool.ClimbingGear, Tool.None);
        private static readonly ImmutableArray<Tool> NarrowRegionTools = ImmutableArray.Create(Tool.Torch, Tool.None);
        private readonly RegionDetailsFactory _factory;

        public Graph(Point2d start, Point2d target, ulong depth)
        {
            _factory = new RegionDetailsFactory(start, target, depth);
        }

        public IEnumerable<Node> Neigbours(Node node)
        {
            var (point, currentTool) = node;

            var currentRegion = _factory.Calculate(point);
            var currentValidTools = GetValidTools(currentRegion.CellType);

            foreach (var otherTool in Tools.Where(t => t != node.Tool && currentValidTools.Contains(t)))
            {
                yield return new Node(node.Position, otherTool);
            }

            foreach (var neighbour in PointHelpers.GetDirectNeighbours(node.Position))
            {
                if (neighbour.X < 0 || neighbour.Y < 0)
                {
                    continue;
                }

                var regionDetails = _factory.Calculate(neighbour);

                var validTools = GetValidTools(regionDetails.CellType);

                if (validTools.Contains(currentTool))
                {
                    yield return new Node(neighbour, node.Tool);
                }
            }
        }

        private static ImmutableArray<Tool> GetValidTools(CellType cellType)
        {
            return cellType switch
            {
                CellType.Rocky => RockyRegionTools,
                CellType.Narrow => NarrowRegionTools,
                CellType.Wet => WetRegionTools,
                _ => throw new NotImplementedException(),
            };
        }

        public int Cost(Node nodeA, Node nodeB)
        {
            if (nodeA.Position != nodeB.Position)
            {
                Debug.Assert(nodeA.Tool == nodeB.Tool);
                return 1;
            }
            else if (nodeA.Tool != nodeB.Tool)
            {
                Debug.Assert(nodeA.Position == nodeB.Position);
                return 7;
            }

            throw new InvalidOperationException("We must have done something, right!?");
        }

        private class RegionDetailsFactory
        {
            private readonly Point2d _start;
            private readonly Point2d _target;
            private readonly ulong _depth;
            private readonly Dictionary<Point2d, RegionDetails> _map;

            public RegionDetailsFactory(Point2d start, Point2d target, ulong depth)
            {
                _start = start;
                _target = target;
                _depth = depth;
                _map = new Dictionary<Point2d, RegionDetails>();
            }

            internal RegionDetails Calculate(Point2d current)
            {
                return RegionDetails.Calculate(_start, _target, current, _depth, _map);
            }
        }
    }

    public interface IGraph
    {
        IEnumerable<Point2d> Neigbours(Point2d node);

        /// <summary>
        /// A standard cost function for a grid using manhattan distance expecting only 4 directions
        /// </summary>
        /// <remarks>
        /// http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
        /// </remarks>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int Cost(Point2d a, Point2d b) => PointHelpers.ManhattanDistance(a, b);
    }
}

