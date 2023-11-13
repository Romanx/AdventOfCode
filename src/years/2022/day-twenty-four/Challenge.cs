using Shared;

namespace DayTwentyFour2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 24), "Blizzard Basin");

    public void PartOne(IInput input, IOutput output)
    {
        var (valley, start, exit) = input.ParseValley();
        var (steps, _) = Solve(start, exit, valley, 0);

        output.WriteProperty("Total Steps", steps);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (valley, start, exit) = input.ParseValley();

        var toGoal = Solve(start, exit, valley, 0);
        var backToStart = Solve(exit, start, toGoal.State, toGoal.Steps);
        var backToGoal = Solve(start, exit, backToStart.State, backToStart.Steps);

        output.WriteProperty("Total Steps", backToGoal.Steps);
    }

    static (uint Steps, Valley State) Solve(
        Point2d start,
        Point2d exit,
        Valley state,
        uint steps)
    {
        var states = new Dictionary<uint, Valley>()
        {
            [steps] = state,
        };

        var currentFrontier = new List<PathAttempt>();
        var nextFrontier = new List<PathAttempt>();
        var seen = new HashSet<PathAttempt>();
        currentFrontier.Add(new PathAttempt(steps, start));

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                // If we've already seen it then skip.
                if (seen.Add(current) is false)
                {
                    continue;
                }

                var next = states.GetOrCalculateIfAbsent(
                    current.Steps + 1,
                    static (step, states) =>
                    {
                        var current = states[step - 1];
                        return current.Next();
                    },
                    states);

                if (next.IsOpen(current.Location))
                {
                    nextFrontier.Add(current.Move());
                }

                var adjacent = current.Location.GetNeighbours(AdjacencyType.Cardinal);

                if (adjacent.Contains(exit))
                {
                    return (current.Steps + 1, next);
                }

                foreach (var neighbour in adjacent)
                {
                    if (neighbour == start)
                        continue;

                    if (next.IsOpen(neighbour) && next.InBounds(neighbour))
                    {
                        nextFrontier.Add(current.Move(neighbour));
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }

        throw new InvalidOperationException("No path to goal");
    }
}
