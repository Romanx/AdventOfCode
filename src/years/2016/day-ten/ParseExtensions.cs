using System;
using System.Collections.Immutable;
using PCRE;
using Shared;

namespace DayTen2016
{
    internal static class ParseExtensions
    {
        private static readonly PcreRegex valueRegex = new("value (?<Value>[0-9]+) goes to bot (?<Bot>[0-9]+)");
        private static readonly PcreRegex assignmentRegex = new("bot (?<Bot>[0-9]+) gives low to (?<LowTarget>bot|output) (?<LowTargetNumber>[0-9]+) and high to (?<HighTarget>bot|output) (?<HighTargetNumber>[0-9]+)");

        public static ImmutableList<Action> Parse(this IInput input)
        {
            var builder = ImmutableList.CreateBuilder<Action>();

            foreach (var line in input.Lines.AsMemory())
            {
                var span = line.Span;

                var valueMatch = valueRegex.Match(span);

                if (valueMatch.Success)
                {
                    builder.Add(new Assignment(
                        int.Parse(valueMatch["Bot"].Value),
                        int.Parse(valueMatch["Value"].Value)
                    ));
                }
                else
                {
                    var assignment = assignmentRegex.Match(span);

                    var bot = int.Parse(assignment["Bot"].Value);
                    var low = new Target(
                        Enum.Parse<TargetType>(assignment["LowTarget"].Value.ToString(), true),
                        int.Parse(assignment["LowTargetNumber"].Value));

                    var high = new Target(
                        Enum.Parse<TargetType>(assignment["HighTarget"].Value.ToString(), true),
                        int.Parse(assignment["HighTargetNumber"].Value));

                    builder.Add(new Transfer(bot, low, high));
                }
            }

            return builder.ToImmutable();
        }
    }
}
