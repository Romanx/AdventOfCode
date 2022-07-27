namespace DayEight2018;

internal static class ParseExtensions
{
    public static Node Parse(this IInput input)
    {
        var span = input.Content.AsSpan();
        return ParseNode(ref span);

        static Node ParseNode(ref ReadOnlySpan<char> span)
        {
            var childQuantity = span.ParseInt();
            var metadataQuantity = span.ParseInt();

            var children = new Node[childQuantity];

            for (var i = 0; i < childQuantity; i++)
            {
                children[i] = ParseNode(ref span);
            }

            var metadata = new int[metadataQuantity];
            for (var i = 0; i < metadataQuantity; i++)
            {
                var meta = span.ParseInt();
                metadata[i] = meta;
            }

            return new Node(children, metadata);
        }
    }

    public static int ParseInt(this ref ReadOnlySpan<char> span)
    {
        var index = 0;

        while (char.IsDigit(span[index]) && index + 1 < span.Length)
        {
            index++;
        }

        var result = index == 0 ? int.Parse(span) : int.Parse(span[..index]);
        span = span[(index + 1)..];
        return result;
    }
}
