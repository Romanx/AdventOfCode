namespace DayThirteen2020
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 13), "Shuttle Search");

        public override void PartOne(IInput input, IOutput output)
        {
            var (earliestTimestamp, busIds) = input.ParseInput();

            var (busId, closestTime) = busIds
                .Select(id =>
                {
                    var rounded = (int)Math.Truncate((decimal)earliestTimestamp / id);

                    return (Id: id, ClosestTime: id * (rounded + 1));
                })
                .OrderBy(v => v.ClosestTime)
                .First();

            var timeWaiting = closestTime - earliestTimestamp;
            output.WriteProperty("Closest Bus", busId);
            output.WriteProperty("Closest Time", closestTime);
            output.WriteProperty("Time Waiting", timeWaiting);
            output.WriteProperty("Answer", timeWaiting * busId);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var buses = input.ParseBuses();

            // We decrement the offset since the remainder of the modulo should be BusId minus the offset.
            var modulos = buses.Select(b => b.BusId - b.Offset).ToArray();
            var numbers = buses.Select(b => b.BusId).ToArray();

            var result = ChineseRemainderTheorem.Solve(numbers, modulos);
            output.WriteProperty("Timestamp", result);
        }
    }

    internal static class ParseExtensions
    {
        public static (int EarliestTimestamp, ImmutableArray<int> BusIds) ParseInput(this IInput input)
        {
            var lines = input.Lines.AsMemory().ToArray();

            int earliestTimestamp = int.Parse(lines[0].Span);

            return (earliestTimestamp, ParseBusIds(lines[1].ToString()));

            static ImmutableArray<int> ParseBusIds(string line)
            {
                return line
                    .Split(",")
                    .Where(l => l != "x")
                    .Select(int.Parse)
                    .ToImmutableArray();
            }
        }

        public static ImmutableArray<(long BusId, long Offset)> ParseBuses(this IInput input)
        {
            var line = input.Lines.AsMemory().Skip(1).First().ToString();

            return line
                .Split(",")
                .Select((el, idx) => (el, idx))
                .Where(x => x.el != "x")
                .Select(x => (long.Parse(x.el), (long)x.idx))
                .ToImmutableArray();
        }
    }
}
