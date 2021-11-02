using System;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using NodaTime;
using Shared;

namespace DayNine2020
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 09), "Encoding Error");

        public override void PartOne(IInput input, IOutput output)
        {
            var longs = input.AsLongs();
            var value = FindIncorrectCandidate(longs);

            output.WriteProperty("Value not matching", value);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var longs = input.AsLongs();
            var target = FindIncorrectCandidate(longs);

            var matchingSubset = FindMatchingSubset(longs, target);

            var smallest = matchingSubset.Min();
            var largest = matchingSubset.Max();

            output.WriteProperty("Smallest", smallest);
            output.WriteProperty("Largest", largest);
            output.WriteProperty("Weakness", smallest + largest);

            static ImmutableArray<long> FindMatchingSubset(long[] longs, long target)
            {
                var startPos = 0;
                var size = 2;
                var span = longs.AsSpan();

                while (true)
                {
                    if (size + startPos > span.Length)
                    {
                        size++;
                        startPos = 0;
                    }

                    var slice = span[startPos..(size + startPos)];
                    if (slice.IsEmpty)
                    {
                    }

                    long sum = 0;
                    foreach (var num in slice)
                    {
                        sum += num;
                    }

                    if (sum == target)
                    {
                        return slice.ToArray().ToImmutableArray();
                    }

                    startPos++;
                }
            }
        }

        private static long FindIncorrectCandidate(long[] longs)
        {
            const int PreambleLength = 25;
            var index = 0;

            foreach (var item in longs[PreambleLength..])
            {
                var currentPreamble = longs[index..(PreambleLength + index)];
                var candidates = currentPreamble.Subsets(2);
                bool matchFound = false;

                foreach (var pair in candidates)
                {
                    if (pair.Sum() == item)
                    {
                        matchFound = true;
                        break;
                    }
                }

                if (matchFound is false)
                {
                    return item;
                }

                index++;
            }

            throw new InvalidOperationException("All values are valid");
        }
    }

    public static class ParsingExtensions
    {
        public static long[] AsLongs(this IInput input) => input.Lines.AsMemory().Select(l => long.Parse(l.Span)).ToArray();
    }
}
