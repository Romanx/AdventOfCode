using System.Collections.Frozen;
using CommunityToolkit.HighPerformance;

namespace DayTwentyTwo2022;

internal static class ParseExtensions
{
    public static (RegionalGame Game, ImmutableArray<Command> Commands) ParseGameFlat(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var paragraph = paragraphs[0];
        var map = SpanHelpers.As2dArray(paragraph);
        var regions = ParseRegionsFlat(map);

        var player = new RegionPlayer(
            new Orientation((0, 0), GridDirection.Right),
            regions[1]);

        var commands = ParseCommands(paragraphs[1].Span[0]);

        return (new RegionalGame(player), commands);
    }

    public static (RegionalGame Game, ImmutableArray<Command> Commands) ParseGameAsCube(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var paragraph = paragraphs[0];
        var map = SpanHelpers.As2dArray(paragraph);
        var regions = ParseRegionsAsCube(map);

        var player = new RegionPlayer(
            new Orientation((0, 0), GridDirection.Right),
            regions[1]);

        var commands = ParseCommands(paragraphs[1].Span[0]);

        return (new RegionalGame(player), commands);
    }

    private static ImmutableArray<Command> ParseCommands(ReadOnlyMemory<char> memory)
    {
        var span = memory.Span;
        var commands = ImmutableArray.CreateBuilder<Command>();

        while (span.IsEmpty is false)
        {
            if (char.IsAsciiDigit(span[0]))
            {
                var idx = 1;
                while (idx < span.Length && char.IsAsciiDigit(span[idx]))
                {
                    idx++;
                }

                commands.Add(new MoveCommand(int.Parse(span[0..idx])));
                span = span[idx..];
            }
            else if (span[0] is 'R' or 'L')
            {
                var dir = GridDirection.FromChar(span[0]);
                commands.Add(new TurnCommand(dir));
                span = span[1..];
            }
            else
            {
                throw new InvalidOperationException("Invalid input for commands");
            }
        }

        return commands.ToImmutableArray();
    }

    private static FrozenDictionary<int, Region> ParseRegionsFlat(char[,] map)
    {
        const int RegionSize = 50;
        var layout = new char[,]
        {
            { ' ', '1', '2' },
            { ' ', '3', ' ' },
            { '4', '5', ' ' },
            { '6', ' ', ' ' },
        }.AsSpan2D();

        var memory = map.AsMemory2D();
        var dictionary = new Dictionary<int, Region>(6);

        for (var y = 0; y < layout.Height; y++)
        {
            for (var x = 0; x < layout.Width; x++)
            {
                var yOffset = y * RegionSize;
                var xOffset = x * RegionSize;

                if (char.IsAsciiDigit(layout[y, x]))
                {
                    var digit = layout[y, x] - '0';

                    var regionRange = memory[
                        yOffset..(yOffset + RegionSize),
                        xOffset..(xOffset + RegionSize)];

                    dictionary[digit] = new Region(
                        digit,
                        regionRange,
                        new(xOffset, yOffset));
                }
            }
        }

        dictionary[1].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[5]),
            new Transition(Direction.East, Direction.East, dictionary[2]),
            new Transition(Direction.South, Direction.South, dictionary[3]),
            new Transition(Direction.West, Direction.West, dictionary[2]),
        ]);
        dictionary[2].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[2]),
            new Transition(Direction.East, Direction.East, dictionary[1]),
            new Transition(Direction.South, Direction.South, dictionary[2]),
            new Transition(Direction.West, Direction.West, dictionary[1]),
        ]);
        dictionary[3].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[1]),
            new Transition(Direction.East, Direction.East, dictionary[3]),
            new Transition(Direction.South, Direction.South, dictionary[5]),
            new Transition(Direction.West, Direction.West, dictionary[3]),
        ]);
        dictionary[4].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[6]),
            new Transition(Direction.East, Direction.East, dictionary[5]),
            new Transition(Direction.South, Direction.South, dictionary[6]),
            new Transition(Direction.West, Direction.West, dictionary[5]),
        ]);
        dictionary[5].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[3]),
            new Transition(Direction.East, Direction.East, dictionary[4]),
            new Transition(Direction.South, Direction.South, dictionary[1]),
            new Transition(Direction.West, Direction.West, dictionary[4]),
        ]);
        dictionary[6].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[4]),
            new Transition(Direction.East, Direction.East, dictionary[6]),
            new Transition(Direction.South, Direction.South, dictionary[4]),
            new Transition(Direction.West, Direction.West, dictionary[6]),
        ]);

        return dictionary.ToFrozenDictionary();
    }

    private static FrozenDictionary<int, Region> ParseRegionsAsCube(char[,] map)
    {
        const int RegionSize = 50;
        var layout = new char[,]
        {
            { ' ', '1', '2' },
            { ' ', '3', ' ' },
            { '4', '5', ' ' },
            { '6', ' ', ' ' },
        }.AsSpan2D();

        var memory = map.AsMemory2D();
        var dictionary = new Dictionary<int, Region>(6);

        for (var y = 0; y < layout.Height; y++)
        {
            for (var x = 0; x < layout.Width; x++)
            {
                var yOffset = y * RegionSize;
                var xOffset = x * RegionSize;

                if (char.IsAsciiDigit(layout[y, x]))
                {
                    var digit = layout[y, x] - '0';

                    var regionRange = memory[
                        yOffset..(yOffset + RegionSize),
                        xOffset..(xOffset + RegionSize)];

                    dictionary[digit] = new Region(
                        digit,
                        regionRange,
                        new(xOffset, yOffset));
                }
            }
        }

        dictionary[1].AddTransitions([
            new Transition(Direction.North, Direction.East, dictionary[6]),
            new Transition(Direction.East, Direction.East, dictionary[2]),
            new Transition(Direction.South, Direction.South, dictionary[3]),
            new Transition(Direction.West, Direction.East, dictionary[4]),
        ]);
        dictionary[2].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[6]),
            new Transition(Direction.East, Direction.West, dictionary[5]),
            new Transition(Direction.South, Direction.West, dictionary[3]),
            new Transition(Direction.West, Direction.West, dictionary[1]),
        ]);
        dictionary[3].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[1]),
            new Transition(Direction.East, Direction.North, dictionary[2]),
            new Transition(Direction.South, Direction.South, dictionary[5]),
            new Transition(Direction.West, Direction.South, dictionary[4]),
        ]);
        dictionary[4].AddTransitions([
            new Transition(Direction.North, Direction.East, dictionary[3]),
            new Transition(Direction.East, Direction.East, dictionary[5]),
            new Transition(Direction.South, Direction.South, dictionary[6]),
            new Transition(Direction.West, Direction.East, dictionary[1]),
        ]);
        dictionary[5].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[3]),
            new Transition(Direction.East, Direction.West, dictionary[2]),
            new Transition(Direction.South, Direction.West, dictionary[6]),
            new Transition(Direction.West, Direction.West, dictionary[4]),
        ]);
        dictionary[6].AddTransitions([
            new Transition(Direction.North, Direction.North, dictionary[4]),
            new Transition(Direction.East, Direction.North, dictionary[5]),
            new Transition(Direction.South, Direction.South, dictionary[2]),
            new Transition(Direction.West, Direction.South, dictionary[1]),
        ]);

        return dictionary.ToFrozenDictionary();
    }
}
