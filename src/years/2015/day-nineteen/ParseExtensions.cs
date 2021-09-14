using System;
using System.Collections.Immutable;
using Shared;
using Spectre.Console;

namespace DayNineteen2015
{
    internal static class ParseExtensions
    {
        public static (ImmutableArray<(string Target, string Replacement)> Replacements, string Molecule) Parse(this IInput input)
        {
            var paragraphs = input.AsParagraphs();

            var replacements = ParseReplacements(paragraphs[0]);

            return (replacements, paragraphs[1].Span[0].ToString());
        }

        private static ImmutableArray<(string Target, string Replacement)> ParseReplacements(ReadOnlyMemory<ReadOnlyMemory<char>> readOnlyMemory)
        {
            const string Separator = "=>";
            var builder = ImmutableArray.CreateBuilder<(string, string)>();

            foreach (var line in readOnlyMemory.Span)
            {
                var span = line.Span;
                var index = span.IndexOf(Separator);
                var key = line.Span[0..index].Trim();
                var value = line.Span[(index + 2)..].Trim();

                builder.Add((new string(key), new string(value)));
            }

            return builder.ToImmutable();
        }
    }
}
