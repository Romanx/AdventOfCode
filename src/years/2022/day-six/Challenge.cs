namespace DaySix2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 6), "Tuning Trouble");

    public void PartOne(IInput input, IOutput output)
    {
        var span = input.Content.AsSpan();

        const int markerLength = 4;

        int markerIdx = -1;
        for (var end = markerLength; end < span.Length; end++)
        {
            var start = end - markerLength;
            var slice = span[start..end];
            if (slice.IsDistinct())
            {
                markerIdx = end;
                break;
            }
        }

        output.WriteProperty("Start of Packet Marker Index", markerIdx);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var span = input.Content.AsSpan();

        const int messageLength = 14;

        int markerIdx = -1;
        for (var end = messageLength; end < span.Length; end++)
        {
            var start = end - messageLength;
            var slice = span[start..end];
            if (slice.IsDistinct())
            {
                markerIdx = end;
                break;
            }
        }

        output.WriteProperty("Start of Message Marker Index", markerIdx);
    }
}
