using System;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Spectre.Console;

namespace DayFifteen2018
{
    internal static class ParseExtensions
    {
        public static Game Parse(this IInput input, uint elfAttackDamage = 3)
        {
            var map = ImmutableDictionary.CreateBuilder<Point2d, CellType>();
            var entities = ImmutableArray.CreateBuilder<Entity>();

            var points = input.As2DPoints();
            foreach (var (point, c) in points)
            {
                map[point] = c switch
                {
                    '#' => CellType.Wall,
                    '.' or 'G' or 'E' => CellType.OpenSpace,
                    _ => throw new System.NotImplementedException(),
                };

                var entity = c switch
                {
                    'G' => new Entity(EntityType.Goblin, point),
                    'E' => new Entity(EntityType.Elf, point, AttackDamage: elfAttackDamage),
                    _ => null
                };

                if (entity is not null)
                {
                    entities.Add(entity);
                }
            }

            return new Game(
                map.ToImmutable(),
                entities.ToImmutable());
        }
    }

    internal static class OutputExtensions
    {
        internal static void WriteOutcome(this IOutput output, Game game, string initalState)
        {
            var team = game.ActiveEntities.GroupBy(ae => ae.EntityType).First().Key switch
            {
                EntityType.Goblin => "Goblins",
                EntityType.Elf => "Elves",
                _ => throw new NotImplementedException()
            };

            var healths = game.ActiveEntities.Select(ae => ae.Health);
            var totalHealth = healths.Sum();
            var result = game.Rounds * totalHealth;
            output.WriteProperty("Round", game.Rounds);
            output.WriteProperty($"HP Remaining {string.Join("+", healths)}", totalHealth);
            output.WriteProperty("Result", result);

            output.WriteBlock(console =>
            {
                console.MarkupLine("[bold yellow]Initial Game State[/]");
                console.WriteLine(initalState);
            });

            output.WriteBlock(console =>
            {
                console.MarkupLine($"[bold yellow]Final State (Round {game.Rounds})[/]");
                console.WriteLine(game!.Print());
                console.MarkupLine($"[bold white]{team} win[/]");
            });
        }
    }
}
