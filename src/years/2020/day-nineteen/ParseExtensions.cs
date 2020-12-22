using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;
using Shared.Helpers;

namespace DayNineteen2020
{
    internal static class ParseExtensions
    {
        private static readonly Regex letterRegex = new("(\\d*): \"(\\w)\"");
        private static readonly Regex compoundRegex = new("(\\d*): ((?:\\d\\s?)*)");
        private static readonly Regex binaryRegex = new("(\\d*): ((?:\\d\\s?)*) \\| ((?:\\d\\s?)*)");

        public static (ImmutableDictionary<int, Rule> Rules, ImmutableArray<string> Messages) Parse(this IInput input)
        {
            ReadOnlyMemory<ReadOnlyMemory<char>> lines = input.AsLines().ToArray();

            var ruleSpan = SpanHelpers.SliceUntilBlankLine(lines, out lines);

            return (ParseRules(ruleSpan.Span), ParseMessages(lines.Span));

            static ImmutableDictionary<int, Rule> ParseRules(ReadOnlySpan<ReadOnlyMemory<char>> lines)
            {
                var builder = ImmutableDictionary.CreateBuilder<int, Rule>();

                foreach (var line in lines)
                {
                    var lineStr = line.ToString();

                    var letterMatch = letterRegex.Match(lineStr);
                    var binaryMatch = binaryRegex.Match(lineStr);
                    var compoundMatch = compoundRegex.Match(lineStr);

                    if (letterMatch.Success)
                    {
                        var number = int.Parse(letterMatch.Groups[1].Value);
                        var letter = letterMatch.Groups[2].Value[0];
                        builder.Add(number, new LetterRule(number, letter));
                    }
                    else if (binaryMatch.Success)
                    {
                        var number = int.Parse(binaryMatch.Groups[1].Value);
                        var lhs = new CompoundRule(number, binaryMatch.Groups[2].Value.Split(' ').Select(int.Parse).ToImmutableArray());
                        var rhs = new CompoundRule(number, binaryMatch.Groups[3].Value.Split(' ').Select(int.Parse).ToImmutableArray());
                        builder.Add(number, new BinaryRule(number, lhs, rhs));
                    }
                    else if (compoundMatch.Success)
                    {
                        var number = int.Parse(compoundMatch.Groups[1].Value);
                        var rules = compoundMatch.Groups[2].Value.Split(' ').Select(int.Parse).ToImmutableArray();
                        builder.Add(number, new CompoundRule(number, rules));
                    }
                    else
                    {
                        throw new InvalidOperationException($"No regex matches for {lineStr}");
                    }
                }

                return builder.ToImmutable();
            }

            static ImmutableArray<string> ParseMessages(ReadOnlySpan<ReadOnlyMemory<char>> lines)
            {
                var builder = ImmutableArray.CreateBuilder<string>();

                foreach (var line in lines)
                    builder.Add(line.ToString());

                return builder.ToImmutable();
            }
        }
    }
}
