using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Helpers.Computer;
using MoreLinq;
using NodaTime;
using Shared;
using Shared2019.Computer;

namespace DayTwentyFive2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 25), "Cryostasis");

        public override void PartOne(IInput input, IOutput output)
        {
            var program = input.Parse();
            var droid = new ScanningDroid(program);
            droid.SeekToAirlock();
            var result = BruteForceDoor(droid, output);
            var passcode = Regex.Match(result, "typing ([0-9]*) on the keypad at the main airlock.").Groups[1].Value;
            output.WriteProperty("Passcode", passcode);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }

        private static string BruteForceDoor(ScanningDroid droid, IOutput output)
        {
            var allItems = droid.GetInventory();
            var allSubsets = allItems.Subsets();
            droid.Drop(allItems);

            foreach (var subset in allSubsets)
            {
                if (subset.Count == 0)
                    continue;

                droid.Take(subset);
                var result = droid.Go(Direction.South);

                if (result is not null && result.Contains("you are ejected back to the checkpoint.") is false)
                {
                    output.WriteProperty("Items Used", string.Join(", ", subset));
                    return result;
                }
                droid.Drop(subset);
            }

            throw new InvalidOperationException("We can't get to the right weight!");
        }
    }

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

        public void SeekToAirlock()
        {
            ScanToCheckpoint(Direction.North);
        }

        public ImmutableHashSet<string> GetInventory()
        {
            var lines = computer.EnqueueCommandAndRun("inv")!
                 .Trim()
                 .Split("\n");

            return ParseDelimitedList("Items in your inventory:", lines);
        }

        public void Drop(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                computer.EnqueueCommandAndRun($"drop {item}");
            }
        }

        public void Take(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                computer.EnqueueCommandAndRun($"take {item}");
            }
        }

        public string Go(Direction direction)
        {
            return computer.EnqueueCommandAndRun(direction.DirectionType.ToString().ToLowerInvariant());
        }

        private bool ScanToCheckpoint(Direction from)
        {
            Position += from;
            var output = computer.Run();
            var state = ParseOutput(output!);
            UpdateGrid(state);
            PickupItems(state.Items);

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

                computer.EnqueueCommand(DirectionCommand(door));
                if (ScanToCheckpoint(door.Reverse()))
                {
                    return true;
                }

                computer.EnqueueCommand(DirectionCommand(door.Reverse()));
                _ = computer.Run();
            }

            return false;

            static string DirectionCommand(Direction dir) => dir.DirectionType.ToString().ToLowerInvariant();
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
                    .Select(i => new Direction(Enum.Parse<DirectionType>(i, true)))
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

        private void PickupItems(ImmutableHashSet<string> items)
        {
            foreach (var item in items)
            {
                var takeResult = computer.EnqueueCommandAndRun($"take {item}");
                if (takeResult is null)
                {
                    throw new InvalidOperationException($"Bad item taken! {item}");
                }
            }
        }

        private void DropItems(ImmutableHashSet<string> items)
        {
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

    record SearchState(string Room, ImmutableHashSet<string> Items, ImmutableArray<Direction> Doors);

    enum TileState
    {
        Explored,
        Unknown
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<long> Parse(this IInput input) => IntcodeParser.Parse(input.Content.AsSpan());
    }
}
