﻿namespace DayTwenty2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 20), "Firewall Rules");

        public void PartOne(IInput input, IOutput output)
        {
            var blacklist = input.Parse()
                .MergeOverlapping()
                .ToImmutableArray();

            var firstRange = blacklist[0];
            var firstIp = firstRange.End + 1;

            output.WriteProperty("First valid IP", firstIp);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var blacklist = input.Parse()
                   .MergeOverlapping()
                   .ToImmutableArray();

            var ips = CalculateGapsInBlacklist(blacklist);

            output.WriteProperty("First valid IP", ips.Count());

            static IEnumerable<uint> CalculateGapsInBlacklist(ImmutableArray<NumberRange<uint>> blacklist)
            {
                var first = blacklist[0];
                foreach (var next in blacklist.Skip(1))
                {
                    var start = first.End + 1;
                    var stop = next.Start;

                    for (var i = start; i < stop; i++)
                        yield return i;

                    first = next;
                }
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<NumberRange<uint>> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<NumberRange<uint>>();
            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(ParseRange(line.Span));
            }

            builder.Sort(static (x, y) => x.Start.CompareTo(y.Start));
            return builder.ToImmutable();

            static NumberRange<uint> ParseRange(ReadOnlySpan<char> span)
            {
                var split = span.IndexOf('-');

                var first = uint.Parse(span[..split]);
                var second = uint.Parse(span[(split + 1)..]);

                return new NumberRange<uint>(first, second);
            }
        }
    }
}
