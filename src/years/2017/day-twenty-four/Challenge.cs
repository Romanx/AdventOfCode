namespace DayTwentyFour2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 24), "Electromagnetic Moat");

        public override void PartOne(IInput input, IOutput output)
        {
            var components = input.Lines.ParseComponents();

            var bridges = MakeBridges(
                components,
                ImmutableArray<Component>.Empty,
                0);

            var best = bridges.MaxBy(bridge => CalculateStrength(bridge));

            output.WriteProperty("Strongest Bridge", string.Join("--", best));
            output.WriteProperty("Strength", CalculateStrength(best));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var components = input.Lines.ParseComponents();

            var bridges = MakeBridges(
                components,
                ImmutableArray<Component>.Empty,
                0);

            var best = bridges
                .MaxBySet(bridge => bridge.Length)
                .MaxBy(bridge => CalculateStrength(bridge));

            output.WriteProperty("Strongest Bridge", string.Join("--", best));
            output.WriteProperty("Strength", CalculateStrength(best));
        }

        private static long CalculateStrength(ImmutableArray<Component> bridge)
        {
            var strength = 0L;
            foreach (var component in bridge)
            {
                strength += component.PortA + component.PortB;
            }

            return strength;
        }

        private static List<ImmutableArray<Component>> MakeBridges(ImmutableArray<Component> components, ImmutableArray<Component> bridge, ushort port)
        {
            var compatable = components
                .Where(c => c.ConnectsTo(port))
                .ToArray();

            return compatable.Length switch
            {
                0 => new() { bridge },
                _ => compatable
                    .SelectMany(c => MakeBridges(
                        components.Remove(c),
                        bridge.Add(c),
                        c.Opposite(port)))
                    .ToList(),
            };
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Component> ParseComponents(this IInputLines lines)
        {
            var components = ImmutableArray.CreateBuilder<Component>();

            foreach (var line in lines.AsMemory())
            {
                var span = line.Span;
                var divider = span.IndexOf('/');

                var component = new Component(
                    ushort.Parse(span[..divider]),
                    ushort.Parse(span[(divider + 1)..]));
                components.Add(component);
            }

            return components.ToImmutable();
        }
    }

    record Component(ushort PortA, ushort PortB)
    {
        public bool ConnectsTo(ushort value)
            => PortA == value || PortB == value;

        internal ushort Opposite(ushort port)
            => port == PortA ? PortB : PortA;

        public override string ToString() => $"{PortA}/{PortB}";
    }
}
