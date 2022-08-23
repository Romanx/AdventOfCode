using Shared.Graph;

namespace DayTwentyFour2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 24), "Air Duct Spelunking");

        public void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();
            var start = map.First(kvp => kvp.Value is PointOfInterest poi && poi.Number == 0).Key;
            var pointsOfInterestCount = map.Count(kvp => kvp.Value is PointOfInterest);

            var path = FindPath(map, GoalFunction, start);

            // Remove Start
            output.WriteProperty("Number of steps", path.Length - 1);

            bool GoalFunction(State input)
            {
                return input.Visited.Count == pointsOfInterestCount;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var map = input.Parse();
            var start = map.First(kvp => kvp.Value is PointOfInterest poi && poi.Number == 0).Key;
            var pointsOfInterestCount = map.Count(kvp => kvp.Value is PointOfInterest);

            var path = FindPath(map, GoalFunction, start);

            // Remove Start
            output.WriteProperty("Number of steps", path.Length - 1);

            bool GoalFunction(State input)
            {
                return input.Visited.Count == pointsOfInterestCount &&
                       input.Position == start;
            }
        }

        static ImmutableArray<Point2d> FindPath(
            ImmutableDictionary<Point2d, Cell> map,
            Func<State, bool> goalReached,
            Point2d start)
        {
            var startState = new State(start, ImmutableSortedSet.Create(0));
            var frontier = new PriorityQueue<State, float>();
            frontier.Enqueue(startState, 0);

            var cameFrom = new Dictionary<State, State>()
            {
                [startState] = startState
            };

            var costSoFar = new Dictionary<State, float>()
            {
                [startState] = 0
            };

            State? finishedState = null;
            while (frontier.TryDequeue(out var current, out _))
            {
                if (goalReached(current))
                {
                    finishedState = current;
                    break;
                }

                foreach (var next in PointHelpers.GetDirectNeighbours(current.Position))
                {
                    if (map.TryGetValue(next, out var nextCell) is false || nextCell.Type is CellType.Wall)
                    {
                        continue;
                    }

                    var newVisited = nextCell is PointOfInterest poi
                        ? current.Visited.Add(poi.Number)
                        : current.Visited;

                    var nextState = new State(next, newVisited);

                    var newCost = costSoFar[current] + 1;
                    if (costSoFar.TryGetValue(nextState, out var nextCost) is false || newCost < nextCost)
                    {
                        costSoFar[nextState] = newCost;
                        frontier.Enqueue(nextState, newCost);
                        cameFrom[nextState] = current;
                    }
                }
            }

            if (finishedState is null)
            {
                return ImmutableArray<Point2d>.Empty;
            }

            return Algorithms.ReconstructPath(startState, finishedState, cameFrom)
                .Select(state => state.Position)
                .ToImmutableArray();
        }

        record State(Point2d Position, ImmutableSortedSet<int> Visited);
    }

    internal static class ParseExtensions
    {
        public static ImmutableDictionary<Point2d, Cell> Parse(this IInput input)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point2d, Cell>();

            foreach (var (point, c) in input.As2DPoints())
            {
                builder.Add(point, c switch
                {
                    '#' => Cell.Wall,
                    '.' => Cell.Empty,
                    _ when char.IsDigit(c) => new PointOfInterest(int.Parse($"{c}")),
                    _ => throw new InvalidOperationException("Unable to find cell type"),
                });
            }

            return builder.ToImmutable();
        }
    }

    enum CellType
    {
        Empty,
        Wall,
        PointOfInterest
    }

    record PointOfInterest(int Number) : Cell(CellType.PointOfInterest)
    {
        public override string ToString() => $"Point of Interest: {Number}";
    }

    record Cell(CellType Type)
    {
        public static Cell Wall { get; } = new Cell(CellType.Wall);

        public static Cell Empty { get; } = new Cell(CellType.Empty);

        public override string ToString() => $"{Type}";
    }
}
