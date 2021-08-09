using System;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Shared.Helpers;
using Spectre.Console;

namespace DayTwentyTwo2018
{
    internal static class ParseExtensions
    {
        public static (ulong Depth, Point2d Target) Parse(this IInput input)
        {
            var lines = input.AsLines().ToArray();
            var first = lines[0];
            var second = lines[1];

            var depth = ulong.Parse(first.Span[(first.Span.IndexOf(':') + 1)..]);
            var target = new Point2d(SpanHelpers.ParseCommaSeparatedList(second.Span[(second.Span.IndexOf(':') + 1)..]));

            return (depth, target);
        }
    }
}
