using System;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace DayEighteen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 18), "Operation Order");

        private static readonly Tokenizer<ArithmeticExpressionToken> tokenizer = BuildTokenizer();

        public override void PartOne(IInput input, IOutput output)
        {
            var lines = input.Lines.AsMemory();

            var sum = 0L;
            foreach (var line in lines)
            {
                var lineStr = line.ToString();
                var result = TokenizeAndEvaluate(lineStr);
                sum += result;
            }

            output.WriteProperty($"Sum of Results:", sum);

            static long TokenizeAndEvaluate(string line)
            {
                var parser = new EvaluatorWithoutPrecidence(tokenizer.Tokenize(line));
                return parser.ParseExpression().Eval();
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var lines = input.Lines.AsMemory();

            var sum = 0L;
            foreach (var line in lines)
            {
                var lineStr = line.ToString();
                var result = TokenizeAndEvaluate(lineStr);
                sum += result;
            }

            output.WriteProperty($"Sum of Results:", sum);

            static long TokenizeAndEvaluate(string line)
            {
                var parser = new EvaluatorWithPrecidence(tokenizer.Tokenize(line));
                return parser.ParseExpression().Eval();
            }
        }

        private static Tokenizer<ArithmeticExpressionToken> BuildTokenizer()
        {
            return new TokenizerBuilder<ArithmeticExpressionToken>()
               .Ignore(Span.WhiteSpace)
               .Match(Character.EqualTo('+'), ArithmeticExpressionToken.Plus)
               .Match(Character.EqualTo('*'), ArithmeticExpressionToken.Times)
               .Match(Character.EqualTo('('), ArithmeticExpressionToken.LParen)
               .Match(Character.EqualTo(')'), ArithmeticExpressionToken.RParen)
               .Match(Numerics.Natural, ArithmeticExpressionToken.Number)
               .Build();
        }
    }
}
