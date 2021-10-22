using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using NodaTime;
using Shared;

namespace DayTwentyTwo2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 22), "Slam Shuffle");

        public override void PartOne(IInput input, IOutput output)
        {
            var cards = Enumerable.Range(0, 10007);

            var steps = input.Parse();
            foreach (var step in steps)
            {
                cards = step switch
                {
                    { TechniqueType: TechniqueType.DealIntoNewStack } => cards.Reverse(),
                    { TechniqueType: TechniqueType.Cut } => CutN(cards, step.Number!.Value),
                    { TechniqueType: TechniqueType.DealWithIncrement } => DealWithIncrementN(cards, step.Number!.Value),
                    _ => throw new InvalidOperationException(),
                };
            }

            int[] final = cards.ToArray();

            output.WriteProperty("Position of 2019", $"{Array.IndexOf(final, 2019)}");

            static T[] CutN<T>(IEnumerable<T> deck, int count)
            {
                var clone = deck.ToArray();
                if (count > 0)
                {
                    var cut = clone[..count];
                    var rest = clone[count..];

                    return rest.Concat(cut).ToArray();
                }
                else
                {
                    var abs = Math.Abs(count);

                    var cut = clone[^abs..];
                    var rest = clone[..^abs];

                    return cut.Concat(rest).ToArray();
                }
            }

            static T[] DealWithIncrementN<T>(IEnumerable<T> deck, int value)
            {
                var clone = deck.ToArray();
                var copy = new T[clone.Length];
                var pos = 0L;
                foreach (var card in clone)
                {
                    copy[pos % clone.Length] = card;
                    pos += value;
                }

                return copy;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const long NUMBER_OF_CARDS = 119315717514047L;
            const long SHUFFLES = 101741582076661L;
            const int TargetIndex = 2020;

            BigInteger a = 1;
            BigInteger b = 0;

            foreach (var step in input.Parse())
            {
                (a, b) = step switch
                {
                    { TechniqueType: TechniqueType.DealIntoNewStack } => (a * -1, NUMBER_OF_CARDS - b - 1),
                    { TechniqueType: TechniqueType.Cut, Number: not null } => (a, NUMBER_OF_CARDS + b - step.Number.Value),
                    { TechniqueType: TechniqueType.DealWithIncrement, Number: not null } => (a * step.Number.Value, b * step.Number.Value),
                    _ => throw new InvalidOperationException(),
                };
            }

            var power = BigInteger.ModPow(a, SHUFFLES, NUMBER_OF_CARDS);
            var bGazillion = b * (BigInteger.ModPow(a, SHUFFLES, NUMBER_OF_CARDS) - 1) * ModuloInverse(a - 1, NUMBER_OF_CARDS) % NUMBER_OF_CARDS;

            var result = (TargetIndex - bGazillion) % NUMBER_OF_CARDS * ModuloInverse(power, NUMBER_OF_CARDS) % NUMBER_OF_CARDS;

            output.WriteProperty($"Value at position {TargetIndex}", result);
            var outputFile = output.File("output.txt");
            outputFile.Write(Encoding.UTF8.GetBytes($"Value at position {TargetIndex}: {result}"));
        }

        private static BigInteger ModuloInverse(BigInteger a, BigInteger n) => BigInteger.ModPow(a, n - 2, n);
    }

    public static class ParsingExtensions
    {
        public static IEnumerable<Technique> Parse(this IInput input)
        {
            foreach (var line in input.Lines.AsMemory())
            {
                if (line.Span.SequenceEqual("deal into new stack"))
                {
                    yield return new Technique(TechniqueType.DealIntoNewStack, null);
                }
                else if (line.Span.StartsWith("cut "))
                {
                    yield return new Technique(TechniqueType.Cut, int.Parse(line.Span[line.Span.IndexOf(" ")..]));
                }
                else if (line.Span.StartsWith("deal with increment"))
                {
                    yield return new Technique(TechniqueType.DealWithIncrement, int.Parse(line.Span[line.Span.LastIndexOf(" ")..]));
                }
            }
        }
    }

    public enum TechniqueType { DealIntoNewStack, Cut, DealWithIncrement };

    public record Technique(TechniqueType TechniqueType, int? Number);
}
