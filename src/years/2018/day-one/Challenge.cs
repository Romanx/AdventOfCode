namespace DayOne2018;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 1), "Chronal Calibration");

    public override void PartOne(IInput input, IOutput output)
    {
        var totalFrequencyShift = input.Lines.Ints()
            .Aggregate(0, (acc, current) => acc + current);

        output.WriteProperty("Total Frequency Shift", totalFrequencyShift);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var shifts = input.Lines.Ints().ToImmutableArray();

        var found = false;
        var frequencies = new HashSet<int>();

        var current = 0;
        while (found is false)
        {
            foreach (var line in shifts)
            {
                current += line;

                if (frequencies.Contains(current))
                {
                    found = true;
                    break;
                }

                frequencies.Add(current);
            }
        }

        output.WriteProperty("First Duplicate Frequency", current);
    }
}
