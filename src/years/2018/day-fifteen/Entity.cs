using System.Text;
using MoreLinq;

namespace DayFifteen2018
{
    record Entity(EntityType EntityType, Point2d Position, int Health = 200, uint AttackDamage = 3)
    {
        public bool IsDead => Health <= 0;

        public Point2d Position { get; private set; } = Position;

        public int Health { get; private set; } = Health;

        public void TakeTurn(Game game)
        {
            var enemyType = Game.GetEnemyType(EntityType);

            // Look for target next to me
            var targets = game.EntitesNextTo(Position, enemyType);

            // If there is no target next to me look for one to path to
            if (targets.Length == 0)
            {
                var path = FindPathToBestEnemyAdjacentSpot(game);

                // If we have a path then step closer
                if (path.Length > 0)
                {
                    Position = path[0];

                    if (path.Length == 1)
                    {
                        targets = game.EntitesNextTo(Position, enemyType);
                    }
                }
            }

            Attack(targets, this);
        }

        public string DisplayString()
        {
            var builder = new StringBuilder();
            builder.Append(EntityType switch
            {
                EntityType.Goblin => 'G',
                EntityType.Elf => 'E',
                _ => throw new NotImplementedException(),
            });
            builder.Append($"({Health})");

            return builder.ToString();
        }

        public void GetHit(uint damage)
        {
            Health -= (int)damage;
        }

        public ImmutableArray<Point2d> FindPathToBestEnemyAdjacentSpot(Game game)
        {
            var enemies = game.Entities
                .Where(ae => ae.IsDead is false)
                .Where(ae => ae.EntityType == Game.GetEnemyType(EntityType))
                .Select(e => e.Position);

            HashSet<Point2d> inRange = new();
            foreach (var target in enemies
                .SelectMany(position => NeighboursInReadingOrder(position))
                .Where(position => IsValidPosition(game, position)))
            {
                inRange.Add(target);
            }

            var queue = new Queue<Point2d>();
            var prevs = new Dictionary<Point2d, Point2d>();
            queue.Enqueue(Position);
            prevs.Add(Position, Point2d.Origin);

            while (queue.TryDequeue(out var point))
            {
                foreach (var neighbour in NeighboursInReadingOrder(point))
                {
                    if (prevs.ContainsKey(neighbour) || IsValidPosition(game, neighbour) is false)
                        continue;

                    queue.Enqueue(neighbour);
                    prevs.Add(neighbour, point);
                }
            }

            var bestPath = inRange
                .Select(target => GetPath(Position, target))
                .Where(t => t.Length > 0)
                .OrderBy(t => t.Length)
                .ThenBy(t => t[^1], ReadingOrderComparer.Instance)
                .FirstOrDefault();

            return bestPath == default
                ? ImmutableArray<Point2d>.Empty
                : bestPath;

            ImmutableArray<Point2d> GetPath(Point2d sourcePosition, Point2d destination)
            {
                if (prevs.ContainsKey(destination) is false)
                {
                    return ImmutableArray<Point2d>.Empty;
                }

                var path = ImmutableArray.CreateBuilder<Point2d>();
                var current = destination;
                while (current != sourcePosition)
                {
                    path.Add(current);
                    current = prevs[current];
                }

                path.Reverse();
                return path.ToImmutable();
            }

            static bool IsValidPosition(Game game, Point2d position) => game.OpenSpaces.Contains(position) &&
                game.ActiveEntities.Any(ae => ae.Position == position) is false;

            static IEnumerable<Point2d> NeighboursInReadingOrder(Point2d point) => PointHelpers.GetDirectNeighbours(point)
                .OrderBy(p => p, ReadingOrderComparer.Instance);
        }

        private static void Attack(ImmutableArray<Entity> targets, Entity entity)
        {
            if (targets.Length == 0)
                return;

            var selectedTarget = targets
                .OrderBy(t => t.Health)
                .ThenBy(t => t.Position, ReadingOrderComparer.Instance)
                .First();

            selectedTarget.GetHit(entity.AttackDamage);
        }
    }
}
