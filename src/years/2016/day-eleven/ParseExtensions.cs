namespace DayEleven2016
{
    internal static class ParseExtensions
    {
        private static readonly PcreRegex generatorRegex = new("(a (?<Material>[a-z]*) generator)+");
        private static readonly PcreRegex microchipRegex = new("(a (?<Material>[a-z]*)-compatible microchip)+");

        public static ImmutableSortedDictionary<int, ImmutableHashSet<Item>> Parse(this IInput input)
        {
            var index = 0;
            var builder = ImmutableSortedDictionary.CreateBuilder<int, ImmutableHashSet<Item>>();

            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(index, ParseItems(line.Span));
                index++;
            }

            return builder.ToImmutable();

            static ImmutableHashSet<Item> ParseItems(ReadOnlySpan<char> line)
            {
                var builder = ImmutableArray.CreateBuilder<Item>();

                foreach (var generatorMatch in generatorRegex.Matches(line))
                {
                    var material = Enum.Parse<Material>(generatorMatch["Material"].Value.ToString(), true);

                    builder.Add(new Generator(material));
                }

                foreach (var microchipMatch in microchipRegex.Matches(line))
                {
                    var material = Enum.Parse<Material>(microchipMatch["Material"].Value.ToString(), true);

                    builder.Add(new Microchip(material));
                }

                return builder.ToImmutableHashSet();
            }
        }
    }
}
