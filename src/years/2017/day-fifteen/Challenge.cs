using Spectre.Console;

namespace DayFifteen2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 15), "Dueling Generators");

        private const int FactorA = 16807;
        private const int FactorB = 48271;

        public override void PartOne(IInput input, IOutput output)
        {
            var (seedA, seedB) = input.Lines.ParseSeeds();

            var generatorA = ToBinary(CreateGenerator(seedA, FactorA));

            var generatorB = ToBinary(CreateGenerator(seedB, FactorB));

            var count = generatorA.Zip(generatorB)
                .Take(40_000_000)
                .Where(items =>
                {
                    var f = items.First.AsSpan();
                    var s = items.Second.AsSpan();

                    return f.SequenceEqual(s);
                })
                .Count();

            output.WriteProperty("Number of Matches", count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var (seedA, seedB) = input.Lines.ParseSeeds();

            var generatorA = ToBinary(CreateGenerator(seedA, FactorA).Where(l => l % 4 == 0));

            var generatorB = ToBinary(CreateGenerator(seedB, FactorB).Where(l => l % 8 == 0));

            var count = generatorA.Zip(generatorB)
                .Take(5_000_000)
                .Where(items =>
                {
                    var f = items.First.AsSpan();
                    var s = items.Second.AsSpan();

                    return f.SequenceEqual(s);
                })
                .Count();

            output.WriteProperty("Number of Matches", count);
        }

        private static IEnumerable<string> ToBinary(IEnumerable<long> input)
        {
            return input
                .Select(static x => ConvertFunc(x));

            static string ConvertFunc(long input)
            {
                var span = Convert
                    .ToString(input, 2)
                    .PadLeft(32, '0')
                    .AsSpan();

                return new string(span[16..]);
            }
        }

        private static IEnumerable<long> CreateGenerator(int seed, int factor)
        {
            long previous = seed;
            while (true)
            {
                var value = (previous * factor) % 2147483647;
                previous = value;
                yield return value;
            }
        }
    }

    internal static class ParseExtensions
    {
        public static (int SeedA, int SeedB) ParseSeeds(this IInputLines lines)
        {
            var arr = lines.AsArray();

            var seedA = int.Parse(PcreRegex.Match(arr[0], @"(\d+)").Groups[1].Value);
            var seedB = int.Parse(PcreRegex.Match(arr[1], @"(\d+)").Groups[1].Value);

            return (seedA, seedB);
        }
    }
}
