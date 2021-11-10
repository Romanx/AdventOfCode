namespace DayTwenty2020
{
    internal static class ParseExtensions
    {
        public static ImmutableArray<Tile> ParseTiles(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Tile>();

            foreach (var paragraph in input.Lines.AsParagraphs())
            {
                builder.Add(ParseBlock(paragraph));
            }

            return builder.ToImmutable();

            static Tile ParseBlock(ReadOnlyMemory<ReadOnlyMemory<char>> block)
            {
                var firstLine = block.Span[0].Span;

                var number = int.Parse(firstLine[(firstLine.IndexOf(' ') + 1)..^1]);

                var picture = SpanHelpers.As2dArray(block[1..]);

                return new Tile(number, picture);
            }
        }
    }
}
