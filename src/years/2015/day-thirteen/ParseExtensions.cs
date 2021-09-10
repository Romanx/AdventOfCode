using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Shared;

namespace DayThirteen2015
{
    internal static class ParseExtensions
    {
        private static readonly Regex regex = new(@"(?<Person>.*) would (?<Direction>.*) (?<Amount>[0-9]*) happiness units by sitting next to (?<Other>.*)\.");

        public static ImmutableDictionary<string, ImmutableDictionary<string, int>> Parse(this IInput input)
        {
            var builder = new Dictionary<string, Dictionary<string, int>>();
            var people = ImmutableHashSet.CreateBuilder<string>();
            foreach (var line in input.AsStringLines())
            {
                var match = regex.Match(line);

                var personA = match.Groups["Person"].Value;
                var personB = match.Groups["Other"].Value;
                var direction = match.Groups["Direction"].Value;
                var amount = int.Parse(match.Groups["Amount"].Value);

                var happiness = direction switch
                {
                    "lose" => amount * -1,
                    "gain" => amount,
                    _ => throw new InvalidOperationException(),
                };

                if (builder.TryGetValue(personA, out var map) is false)
                {
                    builder[personA] = map = new Dictionary<string, int>();
                };

                map.Add(personB, happiness);
            }

            return builder.ToImmutableDictionary(
                k => k.Key,
                v => v.Value.ToImmutableDictionary());
        }
    }
}
