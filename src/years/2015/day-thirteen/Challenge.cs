using static MoreLinq.Extensions.PermutationsExtension;

namespace DayThirteen2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 13), "Knights of the Dinner Table");

        public override void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();

            var bestHappiness = map.Keys.Permutations()
                .MaxBy(possible => CalculateTableHappiness(possible, map))!;

            output.WriteProperty("Table", string.Join(", ", bestHappiness));
            output.WriteProperty("Table Happiness", CalculateTableHappiness(bestHappiness, map));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var map = input.Parse();
            map = AddMe(map);

            var bestHappiness = map.Keys.Permutations()
                .MaxBy(possible => CalculateTableHappiness(possible, map))!;

            output.WriteProperty("Table", string.Join(", ", bestHappiness));
            output.WriteProperty("Table Happiness", CalculateTableHappiness(bestHappiness, map));

            static ImmutableDictionary<string, ImmutableDictionary<string, int>> AddMe(ImmutableDictionary<string, ImmutableDictionary<string, int>> map)
            {
                var results = map.ToBuilder();

                results.Add("Me", map.Keys
                    .ToImmutableDictionary(k => k, _ => 0));

                foreach (var (key, value) in map)
                {
                    var builder = value.ToBuilder();
                    builder.Add("Me", 0);
                    results[key] = builder.ToImmutable();
                }

                return results.ToImmutable();
            }
        }

        static int CalculateTableHappiness(IList<string> table, ImmutableDictionary<string, ImmutableDictionary<string, int>> map)
        {
            var total = 0;
            for (var i = 0; i < table.Count; i++)
            {
                var current = table[i];
                var happiness = map[current];
                var before = table[ActualIndex(i - 1, table)];
                var after = table[ActualIndex(i + 1, table)];

                total += happiness[before];
                total += happiness[after];
            }

            return total;

            static int ActualIndex(int absoluteIndex, IList<string> table)
            {
                if (absoluteIndex < 0)
                {
                    return table.Count + (absoluteIndex % table.Count);
                }

                return absoluteIndex % table.Count;
            }
        }
    }
}
