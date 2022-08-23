using System.Text.RegularExpressions;
using Shared.Grid;

namespace DaySix2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 6), "Probably a Fire Hazard");

        public void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var grid = CreateGrid<bool>();

            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case LightAction.TurnOn:
                        SetLights(grid, instruction.Range.Items, true);
                        break;
                    case LightAction.TurnOff:
                        SetLights(grid, instruction.Range.Items, false);
                        break;
                    case LightAction.Toggle:
                        foreach (var point in instruction.Range.Items)
                        {
                            grid[point] = !grid[point];
                        }

                        break;
                }
            }

            var litLights = grid.Values.Count(l => l == true);
            output.WriteProperty("Number of Lit Lights", litLights);

            static void SetLights(Dictionary<Point2d, bool> grid, IEnumerable<Point2d> points, bool value)
            {
                foreach (var point in points)
                {
                    grid[point] = value;
                }
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var grid = CreateGrid<int>();

            foreach (var instruction in instructions)
            {
                switch (instruction.Action)
                {
                    case LightAction.TurnOn:
                        AdjustLights(grid, instruction.Range.Items, 1);
                        break;
                    case LightAction.TurnOff:
                        AdjustLights(grid, instruction.Range.Items, -1);
                        break;
                    case LightAction.Toggle:
                        foreach (var point in instruction.Range.Items)
                        {
                            grid[point] = grid[point] + 2;
                        }

                        break;
                }
            }

            var totalBrightness = grid.Values.Sum();
            output.WriteProperty("totalBrightness", totalBrightness);

            static void AdjustLights(Dictionary<Point2d, int> grid, IEnumerable<Point2d> points, int adjustment)
            {
                foreach (var point in points)
                {
                    grid[point] = Math.Max(0, grid[point] + adjustment);
                }
            }
        }

        private static Dictionary<Point2d, T> CreateGrid<T>()
            where T : notnull
        {
            var area = Area2d.Create("0,0", "999,999");
            var grid = new Dictionary<Point2d, T>();
            foreach (var item in area.Items)
            {
                grid.Add(item, default!);
            }

            return grid;
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex regex = new(@"(?<Action>.*) (?<Start>\d*,\d*) through (?<End>\d*,\d*)");

        public static ImmutableArray<Instruction> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Instruction>();

            foreach (var line in input.Lines.AsString())
            {
                var matches = regex.Match(line).Groups;

                var action = matches["Action"].Value switch
                {
                    "turn on" => LightAction.TurnOn,
                    "turn off" => LightAction.TurnOff,
                    "toggle" => LightAction.Toggle,
                    _ => throw new InvalidOperationException("Didn't find action"),
                };

                var start = Point2d.Parse(matches["Start"].Value);
                var end = Point2d.Parse(matches["End"].Value);

                builder.Add(new Instruction(action, Area2d.Create(start, end)));
            }

            return builder.ToImmutable();
        }
    }

    record Instruction(LightAction Action, Area2d Range)
    {
        public override string ToString()
            => $"{Action} {Range} -> {Range.Count}";
    }

    enum LightAction
    {
        TurnOn,
        TurnOff,
        Toggle
    }
}
