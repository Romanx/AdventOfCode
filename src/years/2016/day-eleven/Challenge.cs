using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;

namespace DayEleven2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 11), "Radioisotope Thermoelectric Generators");

        public void PartOne(IInput input, IOutput output)
        {
            var floors = input.Parse();

            var initialState = new State(floors);
            var finalState = Solve(initialState);

            output.WriteProperty("Minimum number of steps", finalState.Steps.Length);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var floors = input.Parse();
            var additionalItems = new Item[]
            {
                new Generator(Material.Elerium),
                new Microchip(Material.Elerium),
                new Generator(Material.Dilithium),
                new Microchip(Material.Dilithium),
            };

            floors = floors.SetItem(0, floors[0].Union(additionalItems));

            var initialState = new State(floors);
            var finalState = Solve(initialState);

            output.WriteProperty("Minimum number of steps", finalState.Steps.Length);
        }

        private static State Solve(State initalState)
        {
            var queue = new Queue<State>();
            var duplicates = new HashSet<State>(SmartStateComparer.Instance)
            {
                initalState
            };

            queue.Enqueue(initalState);

            while (queue.TryDequeue(out var state))
            {
                if (state.IsSolved())
                {
                    return state;
                }

                foreach (var next in state.NextStates())
                {
                    if (duplicates.Contains(next))
                    {
                        continue;
                    }
                    else
                    {
                        queue.Enqueue(next);
                        duplicates.Add(next);
                    }
                }
            }

            throw new InvalidOperationException();
        }
    }

    class SmartStateComparer : IEqualityComparer<State>
    {
        public static SmartStateComparer Instance { get; } = new SmartStateComparer();

        public bool Equals(State? x, State? y)
        {
            return GetHashCode(x!) == GetHashCode(y!);
        }

        public int GetHashCode([DisallowNull] State state)
        {
            HashCode hashCode = default;
            hashCode.Add(state.Elevator);

            var map = new Dictionary<Material, int>();

            foreach (var (floor, items) in state.Floors)
            {
                hashCode.Add(floor);
                if (items.Count == 0)
                {
                    hashCode.Add(-1);
                    continue;
                }

                foreach (var item in items)
                {
                    var prefix = item is Generator
                        ? 'G'
                        : 'M';

                    if (map.TryGetValue(item.Material, out var num) is false)
                    {
                        num = map[item.Material] = map.Count;
                    }

                    hashCode.Add(prefix);
                    hashCode.Add(num);
                }
            }

            return hashCode.ToHashCode();
        }
    }

    internal static class OutputExtensions
    {
        public static void AddPanel(this IOutput output, string title, string content)
        {
            output.WriteBlock(() =>
            {
                return new Panel(new Text(content))
                {
                    Header = new PanelHeader(title, Justify.Center)
                };
            });
        }
    }

    record Item(Material Material) : IComparable<Item>
    {
        public int CompareTo(Item? other) =>
            Material.CompareTo(other?.Material);
    }

    record Microchip(Material Material) : Item(Material)
    {
        public override string ToString() => $"Microchip<{EnumHelpers.ToDisplayName(Material)}>";
    }

    record Generator(Material Material) : Item(Material)
    {
        public override string ToString() => $"Generator<{EnumHelpers.ToDisplayName(Material)}>";
    }

    enum Material
    {
        [Display(Name = "PM")]
        Promethium,
        [Display(Name = "CO")]
        Cobalt,
        [Display(Name = "CM")]
        Curium,
        [Display(Name = "RU")]
        Ruthenium,
        [Display(Name = "PU")]
        Plutonium,
        [Display(Name = "H")]
        Hydrogen,
        [Display(Name = "Li")]
        Lithium,
        [Display(Name = "EL")]
        Elerium,
        [Display(Name = "Di")]
        Dilithium,
    }
}
