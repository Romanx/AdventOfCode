namespace DayThirteen2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 13), "Packet Scanners");

        public void PartOne(IInput input, IOutput output)
        {
            var scanners = input.Lines.ParseScanners();

            var cost = scanners
                .Where(scanner => scanner.CaughtAt(0))
                .Sum(scanner => scanner.Severity);

            output.WriteProperty("Total Cost", cost);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var scanners = input.Lines.ParseScanners();

            var delay = Enumerable.Range(0, int.MaxValue)
                .Where(time => scanners.None(scanner => scanner.CaughtAt(time)))
                .First();

            output.WriteProperty("Time to delay", delay);
        }
    }

    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"(?<Layer>\d+): (?<Range>\d+)");

        public static ImmutableList<Layer> ParseScanners(this IInputLines lines)
        {
            var builder = ImmutableList.CreateBuilder<Layer>();

            foreach (var line in lines.AsMemory())
            {
                var match = regex.Match(line.Span);
                var layer = int.Parse(match.Groups["Layer"].Value);
                var range = int.Parse(match.Groups["Range"].Value);

                builder.Add(new Layer(layer, range));
            }

            builder.Sort((x, y) => x.Depth.CompareTo(y.Depth));
            return builder.ToImmutable();
        }
    }

    record Layer(int Depth, int Range)
    {
        public bool CaughtAt(int time)
            => (time + Depth) % ((Range - 1) * 2) == 0;

        public int Severity { get; } = Depth * Range;
    }
}
