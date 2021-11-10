namespace DaySix2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 6), "Memory Reallocation");

        public override void PartOne(IInput input, IOutput output)
        {
            var banks = input.Content.ParseBanks();

            var set = new HashSet<int>
            {
                Hash(banks)
            };

            var loop = 0;
            while (true)
            {
                var next = Redistribute(banks);
                loop++;

                if (set.Add(Hash(next)) is false)
                {
                    break;
                }
                banks = next;
            }

            output.WriteProperty("Number of steps until loop", loop);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var banks = input.Content.ParseBanks();

            var set = new HashSet<int>
            {
                Hash(banks)
            };

            var loop = 0;
            while (true)
            {
                var next = Redistribute(banks);
                loop++;

                banks = next;
                if (set.Add(Hash(banks)) is false)
                {
                    break;
                }
            }

            var index = set.ToList().IndexOf(Hash(banks));
            var diff = loop - index;

            output.WriteProperty("Loop size", diff);
        }

        static ImmutableArray<Bank> Redistribute(ImmutableArray<Bank> banks)
        {
            var bank = banks
                .MaxBySet(b => b.Blocks)
                .OrderByDescending(b => banks.IndexOf(b))
                .First();

            var builder = banks.ToBuilder();
            var index = banks.IndexOf(bank);
            var blocks = bank.Blocks;
            builder[index] = new Bank(0);

            index = (index + 1) % banks.Length;
            while (blocks > 0)
            {
                var item = builder[index];
                builder[index] = item with { Blocks = item.Blocks + 1 };
                blocks--;

                index = (index + 1) % banks.Length;
            }

            return builder.ToImmutable();
        }

        static int Hash(ImmutableArray<Bank> banks)
        {
            HashCode hashcode = default;

            foreach (var bank in banks)
            {
                hashcode.Add(bank);
            }

            return hashcode.ToHashCode();
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Bank> ParseBanks(this IInputContent content)
        {
            return content.Transform(ParseBanksFromContent);

            static ImmutableArray<Bank> ParseBanksFromContent(string str)
            {
                var trimmed = str.Trim();

                return PcreRegex.Split(trimmed, @"\s+")
                    .Select(str => new Bank(uint.Parse(str.AsSpan())))
                    .ToImmutableArray();
            }
        }
    }

    record Bank(uint Blocks);
}
