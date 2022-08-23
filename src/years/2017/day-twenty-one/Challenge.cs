using Microsoft.Toolkit.HighPerformance;

namespace DayTwentyOne2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 21), "Fractal Art");

        public void PartOne(IInput input, IOutput output)
        {
            const int Iterations = 5;
            var pattern = new[,]
            {
                { '.', '#', '.' },
                { '.', '.', '#' },
                { '#', '#', '#' },
            };
            var rules = input.Lines.ParseRules();

            for (var i = 0; i < Iterations; i++)
            {
                pattern = RunStep(pattern, rules);
            }

            output.WriteProperty("Lit Pixels", pattern.AsSpan().Count('#'));
        }

        public void PartTwo(IInput input, IOutput output)
        {
            const int Iterations = 18;
            var pattern = new[,]
            {
                { '.', '#', '.' },
                { '.', '.', '#' },
                { '#', '#', '#' },
            };
            var rules = input.Lines.ParseRules();

            for (var i = 0; i < Iterations; i++)
            {
                pattern = RunStep(pattern, rules);
            }

            output.WriteProperty("Lit Pixels", pattern.AsSpan().Count('#'));
        }

        private static char[,] RunStep(char[,] input, RuleSet rules)
        {
            var size = input.GetLength(0);

            if (size % 2 == 0)
            {
                return ApplyRules(input, rules, 2);
            }
            else if (size % 3 == 0)
            {
                return ApplyRules(input, rules, 3);
            }

            throw new InvalidOperationException("Not a valid input!");

            static char[,] ApplyRules(char[,] input, RuleSet rules, int size)
            {
                var memory = input.AsMemory2D();
                var panels = input.GetLength(0) / size;
                var resultSize = size + 1;
                var newSize = resultSize * panels;

                var result = new char[newSize, newSize];
                var resultSpan = result.AsSpan2D();

                for (var y = 0; y < panels; y++)
                {
                    var yOffset = y * size;
                    var resultYOffset = y * resultSize;

                    for (var x = 0; x < panels; x++)
                    {
                        var xOffset = x * size;
                        var resultXOffset = x * resultSize;

                        var target = resultSpan[resultXOffset..(resultXOffset + resultSize), resultYOffset..(resultYOffset + resultSize)];

                        var xRange = xOffset..(xOffset + size);
                        var yRange = yOffset..(yOffset + size);
                        var slice = memory[xRange, yRange];

                        var match = rules.FindRule(size, slice.Span);
                        match.Target.AsSpan2D().CopyTo(target);
                    }
                }

                return result;
            }
        }
    }

    record RuleSet(ImmutableArray<Rule> TwoByTwo, ImmutableArray<Rule> ThreeByThree)
    {
        public Rule FindRule(int size, Span2D<char> slice)
        {
            var rules = size == 2
                ? TwoByTwo
                : ThreeByThree;

            foreach (var rule in rules)
            {
                if (rule.Source.AsSpan2D().SequenceEqual(slice))
                {
                    return rule;
                }
            }

            throw new InvalidOperationException("No rule matched!");
        }
    }

    sealed record Rule : IEquatable<Rule>
    {
        public Rule(char[,] source, char[,] target)
        {
            Source = source;
            Target = target;
            Size = source.GetLength(0);
        }

        public char[,] Source { get; }

        public char[,] Target { get; }

        public int Size { get; }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(Size);
            hashcode.Add(Source.GetDjb2HashCode());
            hashcode.Add(Target.GetDjb2HashCode());

            return hashcode.ToHashCode();
        }

        public bool Equals(Rule? other)
        {
            return
                other is not null &&
                EqualityComparer<int>.Default.Equals(Size, other.Size) &&
                Source.AsSpan().SequenceEqual(other.Source.AsSpan()) &&
                Target.AsSpan().SequenceEqual(other.Target.AsSpan());
        }
    }
}
