using CommunityToolkit.HighPerformance;
using Shared.Grid;

namespace DayThree2019;

internal static class ParseExtensions
{
    public static (Wire A, Wire B) ParseWires(this IInput input)
    {
        var lines = input.Lines.AsArray();
        var a = ParseWire(lines[0]);
        var b = ParseWire(lines[1]);

        return (a, b);
    }

    private static Wire ParseWire(ReadOnlySpan<char> line)
    {
        var builder = ImmutableArray.CreateBuilder<LineSegment>();
        var position = Point2d.Origin;

        foreach (var command in line.Tokenize(','))
        {
            var direction = GridDirection.FromChar(command[0]);
            var length = int.Parse(command[1..^0]);

            var start = position + direction;

            var end = start + (direction, length - 1);

            builder.Add(LineSegment.Create(start, end));
            position = end;
        }

        return new Wire(builder.ToImmutable());
    }
}
