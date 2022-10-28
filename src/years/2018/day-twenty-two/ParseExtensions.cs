using Spectre.Console;

namespace DayTwentyTwo2018
{
    internal static class ParseExtensions
    {
        public static (ulong Depth, Point2d Target) Parse(this IInput input)
        {
            var lines = input.Lines.AsMemory().ToArray();
            var first = lines[0];
            var second = lines[1];

            var depth = ulong.Parse(first.Span[(first.Span.IndexOf(':') + 1)..]);
            var span = second.Span[(second.Span.IndexOf(':') + 1)..];
            var target = Point2d.Parse(span);

            return (depth, target);
        }
    }
}
