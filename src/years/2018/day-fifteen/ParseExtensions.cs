using System;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Spectre.Console;
using Spectre.Console.Rendering;

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

            output.WriteBlock(() =>
            {
                return new Panel(initalState)
                {
                    Header = new PanelHeader("[bold yellow]Initial Game State[/]")
                };
            });

            output.WriteBlock(() =>
            {
                var rows = new Rows(new IRenderable[] {
                    new Markup(game!.Print()),
                    new Markup($"[bold white]{team} win[/]"),
                });

                return new Panel(rows)
                {
                    Header = new PanelHeader("[bold yellow]Final State (Round {game.Rounds})[/]")
                };
            });
        }
    }
}
