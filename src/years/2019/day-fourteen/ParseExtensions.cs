using CommunityToolkit.HighPerformance;

namespace DayFourteen2019;

internal static class ParseExtensions
{
    public static ImmutableArray<Reaction> Parse(this IInput input)
    {
        return input
            .Lines
            .Transform(ParseReaction)
            .ToImmutableArray();
    }

    private static Reaction ParseReaction(string input)
    {
        var span = input.AsSpan();
        var split = span.IndexOf("=>");

        var inputs = span[..split];
        var output = span[(split + 2)..];

        return new Reaction(ParseChemicals(inputs), ParseChemical(output));
    }

    private static ImmutableArray<Chemical> ParseChemicals(ReadOnlySpan<char> span)
    {
        var builder = ImmutableArray.CreateBuilder<Chemical>();

        foreach (var chemical in span.Tokenize(','))
        {
            builder.Add(ParseChemical(chemical));
        }

        return builder.ToImmutable();
    }

    private static Chemical ParseChemical(ReadOnlySpan<char> span)
    {
        var trimmed = span.Trim();
        var separator = trimmed.IndexOf(' ');
        var quantity = long.Parse(trimmed[..separator]);
        var type = new string(trimmed[(separator + 1)..]);

        return new Chemical(quantity, type);
    }
}
