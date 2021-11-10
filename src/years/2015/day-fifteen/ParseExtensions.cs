using System.Text.RegularExpressions;

namespace DayFifteen2015
{
    internal static class ParseExtensions
    {
        private static readonly Regex regex = new(@"(?<Ingredient>.*): capacity (?<Capacity>.*), durability (?<Durability>.*), flavor (?<Flavor>.*), texture (?<Texture>.*), calories (?<Calories>.*)");

        public static ImmutableArray<Ingredient> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Ingredient>();
            foreach (var line in input.Lines.AsString())
            {
                builder.Add(ParseLine(line));
            }

            return builder.ToImmutable();

            static Ingredient ParseLine(string line)
            {
                var match = regex.Match(line);

                return new Ingredient(
                    match.Groups["Ingredient"].Value,
                    int.Parse(match.Groups["Capacity"].Value),
                    int.Parse(match.Groups["Durability"].Value),
                    int.Parse(match.Groups["Flavor"].Value),
                    int.Parse(match.Groups["Texture"].Value),
                    int.Parse(match.Groups["Calories"].Value));
            }
        }
    }
}
