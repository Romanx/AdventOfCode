using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime;
using Shared;

namespace DaySeven2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 07), "Handy Haversacks");

        public override void PartOne(IInput input, IOutput output)
        {
            var ruleset = input.ParseRules();

            var result = ruleset
                .Where(rule => ExpandRule(ruleset, rule.Value).Any(c => c.BagColour == "shiny gold"))
                .ToImmutableArray();

            output.WriteProperty("Number of bags", result.Length);

            static IEnumerable<BagCountAndColour> ExpandRule(
                ImmutableDictionary<string, ImmutableArray<BagCountAndColour>> ruleset,
                ImmutableArray<BagCountAndColour> containingBags)
            {
                foreach (var contained in containingBags)
                {
                    yield return contained;
                    foreach (var child in ExpandRule(ruleset, ruleset[contained.BagColour]))
                    {
                        yield return child;
                    }
                }
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var ruleset = input.ParseRules();

            var bag = ruleset["shiny gold"];
            var result = CountBagsInBags(ruleset, bag);

            output.WriteProperty("Number of total bags", result);

            static uint CountBagsInBags(
                ImmutableDictionary<string, ImmutableArray<BagCountAndColour>> ruleset,
                ImmutableArray<BagCountAndColour> containingBags)
            {
                uint bagCount = 0;
                foreach (var contained in containingBags)
                {
                    bagCount += contained.Count;
                    bagCount += CountBagsInBags(ruleset, ruleset[contained.BagColour]) * contained.Count;
                }

                return bagCount;
            }
        }
    }

    public static class ParsingExtensions
    {
        private static readonly Regex ruleRegex = new Regex("(.*) contain ([0-9 a-zA-Z,]*)");
        private static readonly Regex bagRegex = new Regex("(\\d) ([a-zA-Z ]*)[,.]?");

        public static ImmutableDictionary<string, ImmutableArray<BagCountAndColour>> ParseRules(this IInput input)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, ImmutableArray<BagCountAndColour>>();
            foreach (var line in input.Lines.AsMemory())
            {
                var match = ruleRegex.Match(line.ToString());

                ImmutableArray<BagCountAndColour> allowedBags;
                if (line.Span.Contains("no other bags", StringComparison.OrdinalIgnoreCase))
                {
                    allowedBags = ImmutableArray<BagCountAndColour>.Empty;
                }
                else
                {
                    allowedBags = GetContainingBags(match.Groups[2].Value);
                }

                builder.Add(NormalizeBagColour(match.Groups[1].Value), allowedBags);
            };

            return builder.ToImmutable();

            static string NormalizeBagColour(string colour) => colour
                .Replace("bags", "")
                .Replace("bag", "")
                .Trim();

            static ImmutableArray<BagCountAndColour> GetContainingBags(string str)
            {
                var bagBuilder = ImmutableArray.CreateBuilder<BagCountAndColour>();
                var matches = bagRegex.Matches(str);

                foreach (Match match in matches)
                {
                    bagBuilder.Add(new BagCountAndColour(
                        uint.Parse(match.Groups[1].Value),
                        NormalizeBagColour(match.Groups[2].Value)));
                }

                return bagBuilder.ToImmutable();
            }
        }
    }

    public record BagRule(string ContainingBagColour, ImmutableArray<BagCountAndColour> AllowedBags);

    public record BagCountAndColour(uint Count, string BagColour);
}
