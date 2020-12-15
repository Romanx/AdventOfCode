using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayFifteen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 15), "Rambunctious Recitation");

        public override void PartOne(IInput input, IOutput output)
        {
            var inputNumbers = input.ParseInts();
            var number = FindNumberSpokenAtTarget(inputNumbers, 2020);
            output.WriteProperty("2020th Spoken", number);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var inputNumbers = input.ParseInts();
            var number = FindNumberSpokenAtTarget(inputNumbers, 30_000_000);
            output.WriteProperty("30,000,000th Spoken", number);
        }

        public static long FindNumberSpokenAtTarget(IEnumerable<long> seed, long targetPoint)
        {
            var whenSpoken = new Dictionary<long, long>();
            var turn = 1L;
            var nextNumber = 0L;

            foreach (var number in seed)
            {
                nextNumber = Speak(ref turn, number, whenSpoken);
            }

            while (turn < targetPoint)
            {
                nextNumber = Speak(ref turn, nextNumber, whenSpoken);
            }

            return nextNumber;

            static long Speak(ref long turn, long number, Dictionary<long, long> whenSpoken)
            {
                whenSpoken.TryGetValue(number, out long result);

                if (result > 0)
                {
                    result = turn - result;
                }

                whenSpoken[number] = turn++;
                return result;
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<long> ParseInts(this IInput input) => input.AsString().Split(",").Select(long.Parse).ToImmutableArray();
    }
}
