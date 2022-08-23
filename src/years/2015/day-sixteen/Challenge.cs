using System.Text;

namespace DaySixteen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 16), "Aunt Sue");

        private static readonly ImmutableDictionary<string, int> TargetProps = new Dictionary<string, int>
        {
            ["children"] = 3,
            ["cats"] = 7,
            ["samoyeds"] = 2,
            ["pomeranians"] = 3,
            ["akitas"] = 0,
            ["vizslas"] = 0,
            ["goldfish"] = 5,
            ["trees"] = 3,
            ["cars"] = 2,
            ["perfumes"] = 1,
        }.ToImmutableDictionary();

        public void PartOne(IInput input, IOutput output)
        {
            var sues = input.Parse();

            var best = sues
                .MaxBy(sue => Score(sue));

            output.WriteProperty("Sue", best);

            static int Score(Sue sue)
            {
                var score = 0;
                foreach (var (name, value) in TargetProps)
                {
                    if (sue.Properties.TryGetValue(name, out var sueValue) && value == sueValue)
                    {
                        score++;
                    }
                }

                return score;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var sues = input.Parse();

            var best = sues
                .MaxBy(sue => Score(sue));

            output.WriteProperty("Sue", best);

            static int Score(Sue sue)
            {
                var score = 0;
                foreach (var (name, value) in TargetProps)
                {
                    if (sue.Properties.TryGetValue(name, out var sueValue))
                    {
                        if ((name == "cats" || name == "trees"))
                        {
                            if (sueValue > value)
                            {
                                score++;
                            }
                        }
                        else if ((name == "pomeranians" || name == "goldfish"))
                        {
                            if (sueValue < value)
                            {
                                score++;
                            }
                        }
                        else if (value == sueValue)
                        {
                            score++;
                        }
                    }
                }

                return score;
            }
        }
    }

    record Sue(int Number, ImmutableDictionary<string, int> Properties)
    {
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Sue {Number}");
            foreach (var (name, value) in Properties)
            {
                builder.AppendLine($"  - {name}: {value}");
            }

            return builder.ToString().Trim();
        }
    }
}
