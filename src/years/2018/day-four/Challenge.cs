namespace DayFour2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 4), "Repose Record");

    public void PartOne(IInput input, IOutput output)
    {
        var shifts = input.Parse();

        var (id, totalTimeAsleep) = shifts
            .GroupBy(shift => shift.GuardId)
            .Select(guard => (Id: guard.Key, TotalTimeAsleep: guard.Sum(shift => shift.TotalTimeAsleep)))
            .OrderByDescending(guard => guard.TotalTimeAsleep)
            .First();

        var sleepyMinute = shifts
            .Where(shift => shift.GuardId == id)
            .SelectMany(shift => shift.SleepTimes.SelectMany(range => range.ToEnumerable()))
            .GroupBy(asleep => asleep)
            .OrderByDescending(x => x.Count())
            .First();

        output.WriteProperty("Sleepy Guard", id);
        output.WriteProperty("Total time asleep", totalTimeAsleep);
        output.WriteProperty("Asleep most on minute", $"{sleepyMinute.Key} ({sleepyMinute.Count()} times)");
        output.WriteProperty("Answer is", id * sleepyMinute.Key);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var shifts = input.Parse();

        var (guardId, minute) = shifts.SelectMany(shift => shift
                .SleepTimes
                .SelectMany(range => range.ToEnumerable())
                .Select(minute => (shift.GuardId, Minute: minute)))
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .First().Key;

        output.WriteProperty("Sleepy Guard", guardId);
        output.WriteProperty("Minute Asleep", minute);
        output.WriteProperty("Answer is", guardId * minute);
    }
}

readonly record struct GuardShift(DateOnly Date, int GuardId, ImmutableArray<NumberRange<int>> SleepTimes)
{
    public int TotalTimeAsleep { get; } = BuildTotalSleepTime(SleepTimes);

    private static int BuildTotalSleepTime(ImmutableArray<NumberRange<int>> sleepTimes)
    {
        var total = 0;
        foreach (var range in sleepTimes)
        {
            total += range.End - range.Start;
        }

        return total;
    }
}
