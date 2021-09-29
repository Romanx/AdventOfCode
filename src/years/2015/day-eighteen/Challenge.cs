using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NodaTime;
using Shared;
using Shared.Grid;
using Spectre.Console;

namespace DayEighteen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 18), "Like a GIF For Your Yard");

        public override void PartOne(IInput input, IOutput output)
        {
            const int Steps = 100;
            var points = input.Parse();
            var area = Area2d.Create(points.Keys);

            for (var i = 0; i < Steps; i++)
            {
                points = Step(points, area);
            }

            var lightsOn = points.Count(p => p.Value == LightState.On);
            output.WriteProperty("Number of lights on", lightsOn);
        }
        public override void PartTwo(IInput input, IOutput output)
        {
            const int Steps = 100;

            var builder = input.Parse().ToBuilder();
            var area = Area2d.Create(builder.Keys);
            LightCorners(area, builder);
            var points = builder.ToImmutable();

            for (var i = 0; i < Steps; i++)
            {
                points = Step(points, area);
            }

            var lightsOn = points.Count(p => p.Value == LightState.On);
            output.WriteProperty("Number of lights on", lightsOn);
        }

        private static ImmutableDictionary<Point2d, LightState> Step(
            ImmutableDictionary<Point2d, LightState> grid, Area2d area)
        {
            var builder = grid.ToBuilder();

            foreach (var item in area.Items)
            {
                var (on, off) = Neighbours(item, grid);

                builder[item] = grid[item] switch
                {
                    LightState.Off when on == 3 => LightState.On,
                    LightState.On when on == 2 || on == 3 => LightState.On,
                    LightState.Off => LightState.Off,
                    LightState.On => LightState.Off,
                    _ => throw new System.NotImplementedException(),
                };
            }

            LightCorners(area, builder);
            return builder.ToImmutable();

            static (int On, int Off) Neighbours(Point2d point, ImmutableDictionary<Point2d, LightState> grid)
            {
                var on = 0;
                var off = 0;

                foreach (var neighbour in point.GetAllNeighbours())
                {
                    if (grid.TryGetValue(neighbour, out var value))
                    {
                        if (value is LightState.On)
                        {
                            on++;
                        }
                        else
                        {
                            off++;
                        }
                    }
                    else
                    {
                        off++;
                    }
                }

                return (on, off);
            }
        }

        private static void LightCorners(Area2d area, ImmutableDictionary<Point2d, LightState>.Builder builder)
        {
            builder[area.TopLeft] = LightState.On;
            builder[area.TopRight] = LightState.On;
            builder[area.BottomLeft] = LightState.On;
            builder[area.BottomRight] = LightState.On;
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableDictionary<Point2d, LightState> Parse(this IInput input)
        {
            return input.As2DPoints()
                .ToImmutableDictionary(k => k.Point, v => EnumHelpers.FromDisplayName<LightState>($"{v.Character}"));
        }
    }

    enum LightState
    {
        [Display(Name = ".")]
        Off,

        [Display(Name = "#")]
        On
    }
}
