using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using Microsoft.Toolkit.HighPerformance.Extensions;
using NodaTime;
using Shared;
using Shared.Helpers;

namespace DaySixteen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 16), "Ticket Translation");

        public override void PartOne(IInput input, IOutput output)
        {
            var (rules, _, nearbyTickets) = input.Parse();

            output.WriteProperty("Ticket Scanning Error Rate", FindTicketScanningErrorRate(rules, nearbyTickets));

            static int FindTicketScanningErrorRate(ImmutableArray<Rule> rules, ImmutableArray<ImmutableArray<int>> nearbyTickets)
            {
                var invalidTotal = 0;

                foreach (var ticket in nearbyTickets)
                {
                    invalidTotal += ticket
                        .Where(field => rules.All(r => !r.Match(field)))
                        .Sum();
                }

                return invalidTotal;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var (rules, myTicket, nearbyTickets) = input.Parse();
            var fieldToIndex = new HashSet<Rule>[rules.Length];

            var onlyValidTickets = OnlyValidTickets(rules, nearbyTickets);

            for (var i = 0; i < rules.Length; i++)
            {
                var list = new HashSet<Rule>();
                var column = onlyValidTickets.GetColumn(i);

                foreach (var rule in rules)
                {
                    if (WholeColumnMatchesRule(column, rule))
                    {
                        list.Add(rule);
                    }
                }

                fieldToIndex[i] = list;
            }

            var fields = ReduceColumnSet(fieldToIndex);

            var multiplied = fields
                .Select((set, idx) => (set.Name, idx))
                .Where(x => x.Name.StartsWith("departure"))
                .Select(x => myTicket[x.idx])
                .Aggregate(1L, (a, b) => a * b);

            output.WriteProperty("Multiplied departure fields", multiplied);

            static int[,] OnlyValidTickets(ImmutableArray<Rule> rules, ImmutableArray<ImmutableArray<int>> nearbyTickets)
            {
                var array = new List<int[]>();

                foreach (var ticket in nearbyTickets)
                {
                    if (ticket.All(field => rules.Any(r => r.Match(field))))
                    {
                        array.Add(ticket.ToArray());
                    }
                }

                return ArrayHelpers.CreateRectangularArray(array);
            }

            static bool WholeColumnMatchesRule(RefEnumerable<int> column, Rule rule)
            {
                foreach (var val in column)
                {
                    if (rule.Match(val) is false)
                    {
                        return false;
                    }
                }

                return true;
            }

            static ImmutableArray<Rule> ReduceColumnSet(HashSet<Rule>[] fieldToIndex)
            {
                var queue = new Queue<Rule>(fieldToIndex.Where(f => f.Count == 1).Select(l => l.First()));

                while (queue.TryDequeue(out var rule))
                {
                    foreach (var field in fieldToIndex.Where(f => f.Count > 1))
                    {
                        if (field.Contains(rule))
                        {
                            field.Remove(rule);
                        }

                        if (field.Count == 1)
                        {
                            queue.Enqueue(field.First());
                        }
                    }
                }

                return fieldToIndex.Select(f => f.First()).ToImmutableArray();
            }
        }
    }
}
