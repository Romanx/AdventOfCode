using System;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Shared.Helpers;

namespace DaySixteen2018
{
    internal static class ParseExtensions
    {
        public static (ImmutableArray<TestCase> Inputs, ImmutableArray<Command> Program) Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<TestCase>();
            var paragraphs = input.AsParagraphs();

            var programIndex = Array.FindIndex(paragraphs, paragraph => paragraph.Span[0].Span.StartsWith("Before:") is false);
            var cases = paragraphs.AsSpan()[..programIndex];
            var program = paragraphs[programIndex];

            foreach (var paragraph in cases)
            {
                builder.Add(ParseTestCase(paragraph));
            }

            return (builder.ToImmutable(), ParseProgram(program));
        }

        private static TestCase ParseTestCase(ReadOnlyMemory<System.ReadOnlyMemory<char>> paragraph)
        {
            var before = ParseList(paragraph.Span[0].Span);
            var command = ToCommand(paragraph.Span[1].Span);
            var after = ParseList(paragraph.Span[2].Span);

            return new TestCase(before, after, command);

            static ImmutableArray<int> ParseList(ReadOnlySpan<char> span)
            {
                var start = span.IndexOf('[') + 1;
                var end = span.LastIndexOf(']');
                return SpanHelpers.ParseCommaSeparatedList(span[start..end]);
            }
        }

        private static ImmutableArray<Command> ParseProgram(ReadOnlyMemory<ReadOnlyMemory<char>> program)
        {
            var builder = ImmutableArray.CreateBuilder<Command>(program.Length);

            foreach (var line in program.Span)
            {
                builder.Add(ToCommand(line.Span));
            }

            return builder.ToImmutable();
        }

        private static Command ToCommand(ReadOnlySpan<char> span)
        {
            var array = span.ToString().Split(' ').Select(i => int.Parse(i)).ToImmutableArray();

            return new Command(array[0], array[1], array[2], array[3]);
        }
    }
}
