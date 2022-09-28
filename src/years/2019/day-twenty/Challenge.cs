using Shared.Graph;

namespace DayTwenty2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 20), "Donut Maze");

    public void PartOne(IInput input, IOutput output)
    {
        var maze = input.Parse(recursive: false);

        var path = Algorithms.BreadthFirstSearch(
            maze,
            maze.Entrance,
            maze.Exit,
            includeStart: false);

        output.WriteProperty("Number of Steps", path.Length);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var maze = input.Parse(recursive: true);

        var path = Algorithms.AStarSearch(
            maze,
            maze.Entrance,
            maze.Exit,
            static (a, b) => PointHelpers.ManhattanDistance(a, b),
            includeStart: false);

        output.WriteProperty("Number of Steps", path.Length);
    }
}
