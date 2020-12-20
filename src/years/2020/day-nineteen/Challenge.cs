using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayNineteen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 19), "Monster Messages");

        public override void PartOne(IInput input, IOutput output)
        {
            var (rules, messages) = input.Parse();

            var expandedRules = ExpandRules(rules);
            var matches = expandedRules[0];

            var count = messages.Count(m => matches.Contains(m));

            output.WriteProperty("Number of Matches", count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var (rules, messages) = input.Parse();

            var expandedRules = ExpandRules(rules);
            var matches = expandedRules[0];

            var matchCount = 0L;
            var firstHalfMatches = new List<(ReadOnlyMemory<char> residual, int countOf31)>();

            // New rules are:
            //   8: 42 | 42 8
            //   11: 42 31 | 42 11 31
            // And our root rule is unchanged:
            //   0: 8 11
            // Combined this means:
            //   - The string needs to end with at least one 31 match
            //   - The string needs to start with at least one more 42 matches than 31 matches

            var rule31 = expandedRules[31];
            var rule42 = expandedRules[42];

            foreach (var message in messages)
            {
                var countOf31 = 0;
                var residual = message.AsMemory();

                while (true)
                {
                    var matching31 = rule31.SingleOrDefault(x => residual.Span.EndsWith(x));
                    if (matching31 == null)
                        break;

                    residual = residual[..(residual.Length - matching31.Length)];
                    countOf31++;
                }

                if (countOf31 > 0)
                {
                    firstHalfMatches.Add((residual, countOf31));
                }
            }

            foreach (var potential in firstHalfMatches)
            {
                var countOf42 = 0;
                var residual = potential.residual;

                while (true)
                {
                    var matching42 = rule42.SingleOrDefault(x => residual.Span.StartsWith(x));
                    if (matching42 == null)
                        break;

                    residual = residual[matching42.Length..];
                    countOf42++;
                }

                if (residual.IsEmpty && countOf42 > potential.countOf31)
                {
                    matchCount++;
                }
            }

            output.WriteProperty("Number of Matches", matchCount);
        }

        private static ImmutableDictionary<int, ImmutableHashSet<string>> ExpandRules(ImmutableDictionary<int, Rule> rules)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, ImmutableHashSet<string>>();
            var remainingRules = rules.ToBuilder();

            foreach (var letterRule in rules.Where(b => b.Value is LetterRule))
            {
                builder[letterRule.Key] = ImmutableHashSet.Create($"{((LetterRule)letterRule.Value).Letter}");
                remainingRules.Remove(letterRule);
            }

            while (remainingRules.Count > 0)
            {
                var (number, rule) = remainingRules.First(r =>
                {
                    if (r.Value is CompoundRule cr)
                    {
                        return !cr.RuleNumbers.Except(builder.Keys).Any();
                    }
                    else if (r.Value is BinaryRule br)
                    {
                        return !br.Lhs.RuleNumbers
                            .Union(br.Rhs.RuleNumbers)
                            .Except(builder.Keys).Any();
                    }

                    throw new InvalidOperationException();
                });

                if (rule is CompoundRule cr)
                {
                    builder[number] = BuildValue(cr.RuleNumbers, builder).ToImmutableHashSet();
                }
                else if (rule is BinaryRule br)
                {
                    var b = ImmutableHashSet.CreateBuilder<string>();
                    b.UnionWith(BuildValue(br.Lhs.RuleNumbers, builder));
                    b.UnionWith(BuildValue(br.Rhs.RuleNumbers, builder));

                    builder[number] = b.ToImmutable();
                }
                remainingRules.Remove(number);
            }

            return builder.ToImmutable();

            static IEnumerable<string> BuildValue(IEnumerable<int> indicies, ImmutableDictionary<int, ImmutableHashSet<string>>.Builder expandedRules)
            {
                var array = indicies.ToArray();
                if (array.Length == 1)
                {
                    return expandedRules[array[0]];
                }

                var result = new List<string>();
                var otherIndices = array[1..];
                foreach (var value in expandedRules[array[0]])
                {
                    foreach (var otherValue in BuildValue(otherIndices, expandedRules))
                    {
                        result.Add(value + otherValue);
                    }
                }

                return result;
            }
        }
    }

    record Rule(int Number);

    record LetterRule(int Number, char Letter) : Rule(Number);

    record CompoundRule(int Number, ImmutableArray<int> RuleNumbers) : Rule(Number);

    record BinaryRule(int Number, CompoundRule Lhs, CompoundRule Rhs) : Rule(Number);
}
