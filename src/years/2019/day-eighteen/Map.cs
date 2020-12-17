using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DayEighteen2019
{
    internal class Map
    {
        private int MaxX;
        private int MinX;
        private int MaxY;
        private int MinY;

        public Map(ImmutableDictionary<Point2d, Cell> cells)
        {
            Cells = cells;
            MaxX = cells.Keys.Max(p => p.Row);
            MinX = cells.Keys.Min(p => p.Row);
            MaxY = cells.Keys.Max(p => p.Column);
            MinY = cells.Keys.Min(p => p.Column);
        }

        public ImmutableDictionary<Point2d, Cell> Cells { get; }

        public ImmutableArray<Cell> Adjacent(Cell cell, ImmutableArray<char> keyChain)
        {
            var points = new[]
            {
                cell.Point + Direction.North,
                cell.Point + Direction.East,
                cell.Point + Direction.South,
                cell.Point + Direction.West,
            };

            var builder = ImmutableArray.CreateBuilder<Cell>(4);

            foreach (var point in points)
            {
                if (point.Row > MaxX || point.Row < MinX)
                {
                    continue;
                }
                else if (point.Column > MaxY || point.Column < MinY)
                {
                    continue;
                }
                else if (Cells[point] is { CellType: CellType.Wall })
                {
                    continue;
                }
                else if (Cells[point] is Door door && keyChain.Contains(door.KeyId) is false)
                {
                    continue;
                }

                builder.Add(Cells[point]);
            }

            return builder.ToImmutable();
        }
    }
}
