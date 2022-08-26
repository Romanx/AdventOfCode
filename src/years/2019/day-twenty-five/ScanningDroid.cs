using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MoreLinq;
using Shared2019.Computer;

namespace DayTwentyFive2019
{
    class ScanningDroid
    {
        private static readonly ImmutableHashSet<string> badItems = ImmutableHashSet.CreateRange(StringComparer.OrdinalIgnoreCase, new[]
        {
            "infinite loop",
            "molten lava",
            "escape pod",
            "giant electromagnet",
            "photons",
        });

        private readonly AsciiComputer computer;
        private readonly Dictionary<Point2d, TileState> grid = new();

        public Point2d Position { get; private set; } = Point2d.Origin;

        public ScanningDroid(ImmutableArray<long> program)
        {
            computer = new AsciiComputer(program);
        }

        public async Task SeekToAirlock()
        {
            var start = await computer.WaitAndReadResult();
            var state = await RoomEntryRoutine(start);

            await ScanToCheckpoint(Direction.North, state);
        }

        public async Task<ImmutableHashSet<string>> GetInventory()
        {
            var lines = (await computer.EnqueueCommandAndRun("inv"))
                 .Trim()
                 .Split("\n");

            return ParseDelimitedList("Items in your inventory:", lines);
        }

        public async Task Drop(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                await computer.EnqueueCommandAndRun($"drop {item}");
            }
        }

        public async Task Take(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                await computer.EnqueueCommandAndRun($"take {item}");
            }
        }

        public async Task<string> Go(Direction direction)
        {
            return await computer.EnqueueCommandAndRun(direction.DirectionType.ToString().ToLowerInvariant());
        }

        private async Task<bool> ScanToCheckpoint(Direction from, SearchState state)
        {
            Position += from;
            if (state.Room.Equals("Security Checkpoint", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var door in state.Doors)
            {
                if (door == from)
                {
                    continue;
                }

                var output = await Go(door);
                state = await RoomEntryRoutine(output);

                if (await ScanToCheckpoint(door.Reverse(), state))
                {
                    return true;
                }

                await Go(door.Reverse());
            }

            return false;
        }

        private async Task<SearchState> RoomEntryRoutine(string output)
        {
            var state = ParseOutput(output);
            UpdateGrid(state);
            await PickupItems(state.Items);
            return state;
        }

        private static SearchState ParseOutput(string output)
        {
            var lines = output.Trim().Split("\n");

            return new SearchState(
                ParseRoom(lines),
                ParseItems(lines),
                ParseDoors(lines));

            static string ParseRoom(string[] lines)
            {
                var roomLine = lines.First(l => l.StartsWith("=="));
                var match = new Regex("== (.*) ==").Match(roomLine);
                return match.Groups[1].Value.Trim();
            }

            static ImmutableHashSet<string> ParseItems(string[] lines)
            {
                var items = ParseDelimitedList("Items here:", lines);
                return items.Except(badItems);
            }

            static ImmutableArray<Direction> ParseDoors(string[] lines)
            {
                var list = ParseDelimitedList("Doors here lead:", lines);
                return list
                    .Select(i => Direction.Parse(i))
                    .OrderBy(dir =>
                    {
                        return dir.DirectionType switch
                        {
                            DirectionType.West => 1,
                            DirectionType.South => 2,
                            DirectionType.East => 3,
                            DirectionType.North => 4,
                            _ => throw new NotImplementedException(),
                        };
                    })
                    .ToImmutableArray();
            }
        }

        private void UpdateGrid(SearchState state)
        {
            grid[Position] = TileState.Explored;
            foreach (var door in state.Doors)
            {
                var nextPosition = Position + door;
                grid.TryAdd(nextPosition, TileState.Unknown);
            }
        }

        private async Task PickupItems(ImmutableHashSet<string> items)
        {
            foreach (var item in items)
            {
                var takeResult = await computer.EnqueueCommandAndRun($"take {item}");
                if (takeResult is null)
                {
                    throw new InvalidOperationException($"Bad item taken! {item}");
                }
            }
        }

        private static ImmutableHashSet<string> ParseDelimitedList(string header, string[] lines)
        {
            var results = ImmutableHashSet.CreateBuilder<string>();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Trim().Equals(header, StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                    for (; i < lines.Length && lines[i].StartsWith('-'); i++)
                    {
                        results.Add(lines[i][2..]);
                    }
                }
            }

            return results.ToImmutable();
        }
    }
}
