using Shared.Graph;
using Spectre.Console;

namespace DayThirteen2016
{
    class OfficeSpace : IWeightedGraph<Point2d>
    {
        private readonly ImmutableDictionary<Point2d, CellType> _office;

        public OfficeSpace(ImmutableDictionary<Point2d, CellType> office)
        {
            _office = office;
        }

        public int Cost(Point2d nodeA, Point2d nodeB) => 1;

        public IEnumerable<Point2d> Neigbours(Point2d node)
        {
            return PointHelpers.GetDirectNeighbours(node)
                .Where(point => point.X >= 0 && point.Y >= 0)
                .Where(point => _office.ContainsKey(point))
                .Where(point => _office[point] is not CellType.Wall);
        }

        public IEnumerable<Point2d> EmptySpaces => _office
            .Where(kvp => kvp.Value is CellType.Empty)
            .Select(kvp => kvp.Key);

        public OfficeSpace SetVisited(IEnumerable<Point2d> points)
        {
            var builder = _office.ToBuilder();
            foreach (var point in points)
            {
                builder[point] = CellType.Visited;
            }

            return new OfficeSpace(builder.ToImmutable());
        }

        public string Print() => GridPrinter.Print(_office);
    }
}
