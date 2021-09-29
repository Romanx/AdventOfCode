using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using static MoreLinq.Extensions.SubsetsExtension;

namespace DayEleven2016
{
    class State : IEquatable<State>
    {
        public State(ImmutableSortedDictionary<int, ImmutableHashSet<Item>> floors)
        {
            Floors = floors;
            Elevator = 0;
            Steps = ImmutableArray<(int FloorDelta, ImmutableHashSet<Item> Items)>.Empty;
        }

        public State(
            ImmutableSortedDictionary<int, ImmutableHashSet<Item>> floors,
            ImmutableArray<(int FloorDelta, ImmutableHashSet<Item> Items)> steps,
            int elevator)
        {
            Floors = floors;
            Elevator = elevator;
            Steps = steps;
        }

        public ImmutableSortedDictionary<int, ImmutableHashSet<Item>> Floors { get; }

        public int Elevator { get; }

        public ImmutableArray<(int FloorDelta, ImmutableHashSet<Item> Items)> Steps { get; }

        public State? MoveTo(int floorDelta, IEnumerable<Item> items)
        {
            var targetFloorNumber = Elevator + floorDelta;

            if (Floors.ContainsKey(targetFloorNumber) is false)
                return null;

            var currentFloor = Floors[Elevator].ToBuilder();
            var targetFloor = Floors[targetFloorNumber].ToBuilder();

            foreach (var item in items)
            {
                currentFloor.Remove(item);
                targetFloor.Add(item);
            }

            var nextFloors = Floors
                .SetItems(new[]
                {
                    KeyValuePair.Create(Elevator, currentFloor.ToImmutable()),
                    KeyValuePair.Create(targetFloorNumber, targetFloor.ToImmutable()),
                });

            var newState = new State(
                nextFloors,
                Steps.Add((floorDelta, items.ToImmutableHashSet())),
                targetFloorNumber
            );

            return newState.IsValid()
                ? newState
                : null;
        }

        public bool IsValid()
        {
            foreach (var (_, items) in Floors)
            {
                var chips = items.OfType<Microchip>();
                var generators = items.OfType<Generator>().ToHashSet();

                if (generators.Count == 0)
                {
                    continue;
                }

                if (chips.Any(chip => generators.Contains(new Generator(chip.Material)) is false))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSolved()
        {
            return Floors.Values.SkipLast(1).All(set => set.Count == 0);
        }

        public IEnumerable<State> NextStates()
        {
            var floorItems = Floors[Elevator];
            foreach (var delta in new[] { -1, 1 })
            {
                foreach (var count in new[] { 1, 2 })
                {
                    foreach (var items in floorItems.Subsets(Math.Min(floorItems.Count, count)))
                    {
                        var newState = MoveTo(delta, items);
                        if (newState is not null)
                            yield return newState;
                    }
                }
            }
        }

        public bool Equals(State? other)
        {
            if (other is null)
                return false;

            if (Elevator.Equals(other.Elevator) is false)
                return false;

            foreach (var (floor, items) in Floors)
            {
                if (other.Floors[floor].SetEquals(items) is false)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as State);

        public override int GetHashCode()
        {
            HashCode hashcode = default;
            hashcode.Add(Elevator);

            foreach (var (_, items) in Floors)
            {
                foreach (var item in items.OrderBy(i => i))
                {
                    hashcode.Add(item);
                }
            }

            return hashcode.ToHashCode();
        }

        public override string ToString()
        {
            var floors = new List<string>();
            foreach (var (i, items) in Floors)
            {
                var levelPart = Elevator == i
                    ? $"[{i + 1}]"
                    : $" {i + 1} ";

                var itemPart = string.Join(' ', items);
                floors.Add($"{levelPart} {itemPart}");
            }

            floors.Reverse();
            return string.Join(Environment.NewLine, floors);
        }

        public string OutputWithLog()
        {
            var builder = new StringBuilder();

            builder.Append(ToString());
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine("Log");

            var log = string.Join(Environment.NewLine, Steps.Select(static (logLine, idx) =>
            {
                return $"{idx + 1}: {logLine.FloorDelta} {string.Join(' ', logLine.Items)}";
            }));
            builder.Append(log);

            return builder.ToString();
        }
    }
}
