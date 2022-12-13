namespace DayThirteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 13), "Distress Signal");

    public void PartOne(IInput input, IOutput output)
    {
        var indicies = 0;

        var pairs = input.ParsePairs();
        for (var i = 0; i < pairs.Length; i++)
        {
            var (left, right) = pairs[i];
            var correctOrder = left.CompareTo(right);

            if (correctOrder is -1)
            {
                indicies += i + 1;
            }
        }

        output.WriteProperty("Sum of correct pair indices", indicies);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var dividerPackets = ImmutableArray.Create(
            Packet.Parse("[[2]]"),
            Packet.Parse("[[6]]"));

        var sorted = input
            .ParsePackets()
            .AddRange(dividerPackets)
            .Sort();

        var decoderKey = 1;
        foreach (var packet in dividerPackets)
        {
            decoderKey *= (sorted.IndexOf(packet) + 1);
        }

        output.WriteProperty("Decoder Key", decoderKey);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<(Packet Left, Packet Right)> ParsePairs(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();

        var packets = ImmutableArray.CreateBuilder<(Packet left, Packet right)>(paragraphs.Length);

        foreach (var paragraph in paragraphs)
        {
            var left = Packet.Parse(paragraph.Span[0].Span);
            var right = Packet.Parse(paragraph.Span[1].Span);

            packets.Add((left, right));
        }

        return packets.MoveToImmutable();
    }

    public static ImmutableArray<Packet> ParsePackets(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var packets = ImmutableArray.CreateBuilder<Packet>(paragraphs.Length * 2);

        foreach (var paragraph in paragraphs)
        {
            var left = Packet.Parse(paragraph.Span[0].Span);
            var right = Packet.Parse(paragraph.Span[1].Span);

            packets.Add(left);
            packets.Add(right);
        }

        return packets.MoveToImmutable();
    }
}
