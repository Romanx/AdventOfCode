using Shared.Grid;

namespace DayTwentyThree2022;

class Simulation(ImmutableHashSet<Point2d> elves)
{
    private HashSet<Point2d> current = [.. elves];
    private HashSet<Point2d> next = new(elves.Count);
    private Dictionary<Point2d, List<Point2d>> cache = new(elves.Count);
    private Area2d area;

    private readonly Direction[][] Directions = [
        [Direction.North, Direction.South, Direction.West, Direction.East],
        [Direction.South, Direction.West, Direction.East, Direction.North],
        [Direction.West, Direction.East, Direction.North, Direction.South],
        [Direction.East, Direction.North, Direction.South, Direction.West],
    ];

    public int RoundNumber { get; private set; }

    public bool Round()
    {
        var areaBuilder = Area2d.CreateBuilder();
        var moved = 0;
        var directions = Directions[RoundNumber % Directions.Length];

        foreach (var elf in current)
        {
            var adjacent = new AdjacentPoints2d(elf, current, AdjacencyType.All);
            bool added = false;

            // No elves adjacent
            if (adjacent.Count is 0)
            {
                StayStill(elf, cache);
                continue;
            }
            else
            {
                foreach (var direction in directions)
                {
                    if (adjacent.Any(direction) is false)
                    {
                        moved++;
                        Move(elf, direction, cache);
                        added = true;
                        break;
                    }
                }

                if (added is false)
                {
                    StayStill(elf, cache);
                }
            }
        }

        foreach (var (target, elves) in cache)
        {
            if (elves.Count is 1)
            {
                next.Add(target);
                areaBuilder.Add(target);
            }
            else
            {
                foreach (var elf in elves)
                {
                    next.Add(elf);
                }
                areaBuilder.AddRange(elves);
            }
        }

        (current, next) = (next, current);
        next.Clear();
        cache.Clear();
        RoundNumber++;
        area = areaBuilder.Build();
        return moved > 0;

        static void StayStill(Point2d elf, Dictionary<Point2d, List<Point2d>> cache) =>
            cache.AddOrUpdate(
                elf,
                static (key, state) => [state],
                static (key, existing, state) => { existing.Add(state); },
                elf);

        static void Move(Point2d elf, Direction direction, Dictionary<Point2d, List<Point2d>> cache)
        {
            var next = elf + direction;
            cache.AddOrUpdate(
                next,
                static (key, state) => [state],
                static (key, existing, state) => { existing.Add(state); },
                elf);
        }
    }

    public string Print() => GridPrinter.Print(current, '#');

    public long EmptySpaces => area.Count - current.Count;
}

