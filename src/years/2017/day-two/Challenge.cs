using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NodaTime;
using PCRE;
using Shared;

namespace DayTwo2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 2), "Corruption Checksum");

        public override void PartOne(IInput input, IOutput output)
        {
            var checksum = 0;

            foreach (var line in input.Lines.AsSeparatedInts())
            {
                var (min, max) = line.Aggregate(
                    (int?)null, (s, e) => s is int n ? Math.Min(n, e) : e,
                    (int?)null, (s, e) => s is int n ? Math.Max(n, e) : e,
                    (min, max) => (min, max));

                checksum += (max!.Value - min!.Value);
            }

            output.WriteProperty("Checksum", checksum);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var results = new List<int>();

            foreach (var line in input.Lines.AsSeparatedInts())
            {
                for (var i = 0; i < line.Length; i++)
                {
                    var item = line[i];
                    for (var y = 0; y < line.Length; y++)
                    {
                        if (y == i)
                        {
                            continue;
                        }

                        var other = line[y];

                        if (item % other == 0)
                        {
                            results.Add(item / other);
                            goto End;
                        }
                    }
                }
End:;
            }

            output.WriteProperty($"Results: '{string.Join(" + ", results)}'", results.Sum());
        }
    }

    internal static class ParseExtensions
    {
        public static IEnumerable<int[]> AsSeparatedInts(this IInputLines lines)
        {
            var regex = new PcreRegex(@"(\d+)\s?", PcreOptions.Compiled);

            return lines.Transform(line =>
            {
                var matches = regex.Matches(line);

                return matches.Select(m => int.Parse(m.Value)).ToArray();
            });
        }
    }
}
