using MoreLinq;

namespace DayFifteen2018
{
    class Game
    {
        private readonly ImmutableDictionary<Point2d, CellType> _map;

        public ImmutableArray<Entity> Entities { get; }

        public ImmutableHashSet<Point2d> OpenSpaces { get; }

        public int Rounds { get; private set; }

        public IEnumerable<Entity> ActiveEntities => Entities.Where(e => e.IsDead is false);

        public Game(
            ImmutableDictionary<Point2d, CellType> map,
            ImmutableArray<Entity> entities)
        {
            _map = map;
            OpenSpaces = _map
                .Where(kvp => kvp.Value is CellType.OpenSpace)
                .Select(kvp => kvp.Key)
                .ToImmutableHashSet();
            Entities = entities;
        }

        public bool Step()
        {
            var entities = Entities
                .OrderBy(i => i.Position, ReadingOrderComparer.Instance)
                .ToImmutableArray();

            foreach (var entity in entities)
            {
                if (entity.IsDead)
                    continue;

                if (NoTargetsExist())
                {
                    return false;
                }

                entity.TakeTurn(this);
            }

            Rounds++;
            return true;

            bool NoTargetsExist() => Entities
                .Where(ae => ae.IsDead is false)
                .GroupBy(ae => ae.EntityType)
                .Count() == 1;
        }

        public string Print() => Printer.Print(_map, ActiveEntities);

        public void RunGameToEnd()
        {
            Rounds = 0;
            while (true)
            {
                if (Step() is false)
                    break;
            }
        }

        public ImmutableArray<Entity> EntitesNextTo(Point2d position, EntityType entityType)
        {
            return position.GetNeighbours(AdjacencyType.Cardinal)
                .SelectMany(pos => ActiveEntities.Where(ae => ae.EntityType == entityType && ae.Position == pos))
                .ToImmutableArray();
        }

        public static EntityType GetEnemyType(EntityType type)
        {
            return type switch
            {
                EntityType.Goblin => EntityType.Elf,
                EntityType.Elf => EntityType.Goblin,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
