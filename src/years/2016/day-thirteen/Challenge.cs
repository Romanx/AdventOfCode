using System.ComponentModel.DataAnnotations;
using Shared.Graph;
using Spectre.Console;

namespace DayThirteen2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 13), "A Maze of Twisty Little Cubicles");

        public override void PartOne(IInput input, IOutput output)
        {
            var graph = input.Parse();

            var path = graph.AStarSearch(
                new Point2d(1, 1),
                new Point2d(31, 39),
                static (a, b) => PointHelpers.ManhattanDistance(a, b));

            output.WriteBlock(() =>
            {
                var visitedOfficeSpace = graph.SetVisited(path);
                return new Panel(visitedOfficeSpace.Print());
            });

            output.WriteProperty("Path length", path.Length - 1);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var graph = input.Parse();

            var start = new Point2d(1, 1);

            var visited = graph.FloodFill(
               start,
               50);

            output.WriteBlock(() =>
            {
                var visitedOfficeSpace = graph.SetVisited(visited);
                return new Panel(visitedOfficeSpace.Print());
            });

            output.WriteProperty("Unique locations within 50 steps", visited.Count);
        }
    }

    enum CellType
    {
        [Display(Name = ".")]
        Empty,
        [Display(Name = "#")]
        Wall,
        [Display(Name = "O")]
        Visited,
    }
}
