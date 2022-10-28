using System.Text;

namespace DayFifteen2018
{
    static class Printer
    {
        public static string Print(ImmutableDictionary<Point2d, CellType> map, IEnumerable<Entity> activeEntities)
        {
            var builder = new StringBuilder();

            var (rows, columns) = PointHelpers.FindSpaceOfPoints(map.Keys);

            foreach (var column in columns)
            {
                foreach (var row in rows)
                {
                    var point = new Point2d(row, column);

                    var cell = map[point];
                    var entities = activeEntities.Where(entity => entity.Position == point).ToArray();
                    if (entities.Length > 1)
                    {
                        throw new InvalidOperationException("More than one entity in the same location!");
                    }
                    var entity = entities.FirstOrDefault();

                    builder.Append(cell switch
                    {
                        CellType.Wall => '#',
                        CellType.OpenSpace when entity is not null => PrintEntity(entity),
                        CellType.OpenSpace => '.',
                        _ => throw new NotImplementedException(),
                    });
                }
                builder.Append("    ");

                var lineEntities = activeEntities
                    .Where(e => e.Position.Column == column)
                    .OrderBy(e => e.Position, ReadingOrderComparer.Instance);

                builder.Append(PrintEntityHealth(lineEntities));
                builder.AppendLine();
            }

            return builder.ToString();

            static char PrintEntity(Entity entity)
            {
                return entity.EntityType switch
                {
                    EntityType.Goblin => 'G',
                    EntityType.Elf => 'E',
                    _ => throw new NotImplementedException(),
                };
            }

            static string PrintEntityHealth(IEnumerable<Entity> entities)
            {
                return string.Join(", ", entities.Select(e => e.DisplayString()));
            }
        }
    }
}
