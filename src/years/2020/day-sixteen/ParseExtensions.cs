using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Shared;

namespace DaySixteen2020
{
    record Data(ImmutableArray<Rule> Rules, ImmutableArray<int> MyTicket, ImmutableArray<ImmutableArray<int>> NearbyTickets);

    record Rule(string Name, ImmutableArray<Range> ValueRanges)
    {
        internal bool Match(int field)
        {
            foreach (var range in ValueRanges)
            {
                if (range.Contains(field))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex RuleRegex = new Regex("(.+): (\\d+)-(\\d+) or (\\d+)-(\\d+)");

        public static Data Parse(this IInput input)
        {
            var lines = input.Lines.AsMemory().ToArray().AsSpan();
            var firstChunk = SliceUntilBlankLine(lines, out lines);
            var secondChunk = SliceUntilBlankLine(lines, out lines);
            var thirdChunk = SliceUntilBlankLine(lines, out lines);

            var rules = ParseRules(firstChunk);
            var myTicket = ParseTicket(secondChunk[1]);
            var nearbyTickets = ParseNearbyTickets(thirdChunk[1..]);

            return new Data(rules, myTicket, nearbyTickets);

            static ImmutableArray<Rule> ParseRules(Span<ReadOnlyMemory<char>> lines)
            {
                var rules = ImmutableArray.CreateBuilder<Rule>();

                foreach (var line in lines)
                {
                    var match = RuleRegex.Match(line.ToString());

                    var name = match.Groups[1].Value;
                    var rangeOne = new Range(
                        int.Parse(match.Groups[2].Value),
                        int.Parse(match.Groups[3].Value));
                    var rangeTwo = new Range(
                        int.Parse(match.Groups[4].Value),
                        int.Parse(match.Groups[5].Value));

                    rules.Add(new Rule(name, ImmutableArray.Create(rangeOne, rangeTwo)));
                }

                return rules.ToImmutable();
            }

            static ImmutableArray<ImmutableArray<int>> ParseNearbyTickets(Span<ReadOnlyMemory<char>> lines)
            {
                var nearbyTickets = ImmutableArray.CreateBuilder<ImmutableArray<int>>();
                foreach (var line in lines)
                {
                    nearbyTickets.Add(ParseTicket(line));
                }

                return nearbyTickets.ToImmutable();
            }

            static ImmutableArray<int> ParseTicket(ReadOnlyMemory<char> line)
            {
                return line.ToString()
                    .Split(",")
                    .Select(int.Parse)
                    .ToImmutableArray();
            }

            static Span<ReadOnlyMemory<char>> SliceUntilBlankLine(Span<ReadOnlyMemory<char>> lines, out Span<ReadOnlyMemory<char>> sliced)
            {
                var index = 0;
                foreach (var line in lines)
                {
                    if (line.IsEmpty)
                    {
                        sliced = lines[(index + 1)..];
                        return lines[..index];
                    }
                    index++;
                }

                sliced = Span<ReadOnlyMemory<char>>.Empty;
                return lines;
            }
        }
    }
}
