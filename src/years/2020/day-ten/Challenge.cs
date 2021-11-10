namespace DayTen2020
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 10), "Adapter Array");

        public override void PartOne(IInput input, IOutput output)
        {
            var differences = new List<int>();
            var finalOrder = ImmutableArray.CreateBuilder<Adapter>();

            var adapters = input
                .ParseAdapters()
                .OrderBy(a => a.Voltage)
                .ToImmutableArray();

            var voltage = 0;
            while (finalOrder.Count != adapters.Length)
            {
                var next = adapters.Where(a => a.InputVoltageRange.Contains(voltage)).OrderBy(a => a.Voltage).First();
                finalOrder.Add(next);
                differences.Add(next.Voltage - voltage);
                voltage = next.Voltage;
            }

            differences.Add(3);

            var differenceMap = differences
                .GroupBy(k => k)
                .ToDictionary(k => k.Key, v => v.Count());

            output.WriteProperty("Number of 1 Volt Differences", differenceMap[1]);
            output.WriteProperty("Number of 3 Volt Differences", differenceMap[3]);
            output.WriteProperty("Multiplied Volt Differences", differenceMap[1] * differenceMap[3]);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var adapters = input
                .ParseAdapters()
                .Select(a => (long)a.Voltage)
                .ToHashSet();

            var targetValue = adapters.Max() + 3;
            adapters.Add(targetValue);
            var cache = new Dictionary<long, long>();

            var permutations = CountPermutations(0);

            output.WriteProperty("Number of permutations", permutations);
            long CountPermutations(long currentVoltage)
            {
                if (cache.TryGetValue(currentVoltage, out var cachedValue))
                {
                    return cachedValue;
                }

                if (currentVoltage == targetValue)
                {
                    return 1;
                }

                long permutations = 0;
                for (var step = 1; step <= 3; step++)
                {
                    var target = currentVoltage + step;
                    if (adapters.Contains(target))
                    {
                        permutations += CountPermutations(target);
                    }
                }

                cache[currentVoltage] = permutations;
                return permutations;
            }
        }
    }

    internal static class ParsingExtensions
    {
        public static IEnumerable<Adapter> ParseAdapters(this IInput input) => input
            .Lines
            .AsMemory()
            .Select(l => new Adapter(int.Parse(l.Span)));
    }

    internal record Adapter(int Voltage)
    {
        public Range InputVoltageRange { get; } = new Range(Math.Max(Voltage - 3, 0), Voltage - 1);
    }
}
