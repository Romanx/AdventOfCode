namespace DayFour2016
{
    internal static class ParseExtensions
    {
        public static ImmutableArray<Room> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Room>();
            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(ParseRoom(line.Span));
            }

            return builder.ToImmutable();
        }

        private static Room ParseRoom(ReadOnlySpan<char> span)
        {
            var bracket = span.IndexOf('[');
            var checksum = span[(bracket + 1)..^1];

            var sectorStart = bracket - 1;
            while (char.IsDigit(span[sectorStart]))
            {
                sectorStart--;
            }
            var sector = span[(sectorStart + 1)..bracket];

            return new Room(
                new string(span[..sectorStart]),
                int.Parse(sector),
                new string(checksum));
        }
    }
}
