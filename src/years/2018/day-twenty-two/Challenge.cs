using System.ComponentModel.DataAnnotations;
using Shared.Graph;
using Spectre.Console;

namespace DayTwentyTwo2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 22), "Mode Maze");

        public override void PartOne(IInput input, IOutput output)
        {
            var start = Point2d.Origin;
            var (depth, target) = input.Parse();

            var map = MapGenerator.Generate(start, target, depth);

            var riskLevel = map.Aggregate(0, (i, item) =>
            {
                return i + item.Value.CellType switch
                {
                    CellType.Rocky => 0,
                    CellType.Wet => 1,
                    CellType.Narrow => 2,
                    _ => 0,
                };
            });

            output.WriteProperty("Risk Level", riskLevel);
            output.WriteBlock(() =>
            {
                return new Table()
                    .HeavyBorder()
                    .AddColumn("Map")
                    .AddRow(Print(start, target, map));
            });
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var start = Point2d.Origin;
            var (depth, target) = input.Parse();

            var graph = new Graph(start, target, depth);
            var path = graph.AStarSearch(
                new Node(start, Tool.Torch),
                new Node(target, Tool.Torch),
                Heuristic);

            var actions = CalculatePath(path);

            var actionTotals = actions
                .GroupBy(a => a)
                .ToDictionary(a => a.Key, v => v.Count() * v.Key switch
                {
                    Action.Move => 1,
                    Action.ChangeTool => 7,
                    _ => throw new NotImplementedException(),
                });

            var timeMoving = actionTotals[Action.Move];
            var timeChangingTools = actionTotals[Action.ChangeTool];
            var timeTaken = timeMoving + timeChangingTools;

            output.WriteProperty("Time taken", timeTaken);
            output.WriteProperty("Time moving", timeMoving);
            output.WriteProperty("Time changing tools", timeChangingTools);

            static IEnumerable<Action> CalculatePath(ImmutableArray<Node> path)
            {
                var current = path[0];
                foreach (var step in path.Skip(1))
                {
                    if (current.Position == step.Position)
                    {
                        yield return Action.ChangeTool;
                    }
                    else
                    {
                        yield return Action.Move;
                    }

                    current = step;
                }
            }

            static float Heuristic(Node goal, Node next)
            {
                return PointHelpers.ManhattanDistance(goal.Position, next.Position);
            }
        }

        private static string Print(Point2d start, Point2d target, ImmutableDictionary<Point2d, RegionDetails> map)
        {
            var printRepresentation = map.ToImmutableDictionary(k => k.Key, v =>
            {
                if (v.Key == start)
                {
                    return CellType.Mouth;
                }
                else if (v.Key == target)
                {
                    return CellType.Target;
                }

                return v.Value.CellType;
            });

            return GridPrinter.Print(printRepresentation);
        }
    }

    enum Action
    {
        Move,
        ChangeTool
    }

    public enum Tool
    {
        None,
        Torch,
        ClimbingGear
    }

    public enum CellType
    {
        [Display(Name = "!")]
        NotSet,
        [Display(Name = "M")]
        Mouth,
        [Display(Name = "T")]
        Target,
        [Display(Name = ".")]
        Rocky,
        [Display(Name = "|")]
        Narrow,
        [Display(Name = "=")]
        Wet
    }

    internal record Node(Point2d Position, Tool Tool);
}
