using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Shared.Helpers;

namespace DayTwentyOne2017
{
    internal static class ParseExtensions
    {
        private static readonly ImmutableArray<Func<char[,], char[,]>> transforms = ImmutableArray.Create<Func<char[,], char[,]>>(
            tile => tile,
            tile => ArrayHelpers.RotateRight(tile),
            tile => ArrayHelpers.RotateRight(tile),
            tile => ArrayHelpers.RotateRight(tile),
            tile => ArrayHelpers.FlipHorizontal(tile),
            tile => ArrayHelpers.RotateRight(tile),
            tile => ArrayHelpers.RotateRight(tile),
            tile => ArrayHelpers.RotateRight(tile));

        public static RuleSet ParseRules(this IInputLines lines)
        {
            var twoByTwo = ImmutableArray.CreateBuilder<Rule>();
            var threeByThree = ImmutableArray.CreateBuilder<Rule>();

            foreach (var line in lines.AsString())
            {
                var split = line.Split("=>", StringSplitOptions.TrimEntries);

                var length = split[0].Length;
                var rules = ParseRule(split[0], split[1])
                    .Distinct();

                if (length == 5)
                {
                    twoByTwo.AddRange(rules);
                }
                else if (length == 11)
                {
                    threeByThree.AddRange(rules);
                }
            }

            return new RuleSet(
                twoByTwo.ToImmutable(),
                threeByThree.ToImmutable());
        }

        private static IEnumerable<Rule> ParseRule(string part1, string part2)
        {
            var source = ParseToArray(part1);
            var target = ParseToArray(part2);

            foreach (var transform in transforms)
            {
                source = transform(source);
                yield return new Rule(source, target);
            }

            static char[,] ParseToArray(string input)
            {
                var split = input
                    .Split('/')
                    .Select(x => x.ToCharArray())
                    .ToArray();

                return ArrayHelpers.CreateRectangularArray(split);
            }
        }
    }
}
