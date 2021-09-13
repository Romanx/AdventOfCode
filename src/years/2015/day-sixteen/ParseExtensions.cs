using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace DaySixteen2015
{
    internal static class ParseExtensions
    {
        private static readonly Regex regex = new("Sue (?<Number>[0-9]*): (?<Props>.*)");

        public static ImmutableArray<Sue> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Sue>();

            foreach (var line in input.AsStringLines())
            {
                var match = regex.Match(line);
                var number = int.Parse(match.Groups["Number"].Value);

                var props = match.Groups["Props"].Value
                    .Split(',')
                    .Select(i =>
                    {
                        var parts = i.Split(':', StringSplitOptions.TrimEntries);
                        return KeyValuePair.Create(parts[0], int.Parse(parts[1]));
                    })
                    .ToImmutableDictionary();

                builder.Add(new Sue(number, props));
            }

            return builder.ToImmutable();
        }
    }
}
