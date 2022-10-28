namespace DayTwentyFive2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 25), "Four-Dimensional Adventure");

        public void PartOne(IInput input, IOutput output)
        {
            var points = input.Parse();

            var constellations = Constellations(points);

            output.WriteProperty("Constellation Count", constellations.Length);
        }

        private static ImmutableArray<ImmutableHashSet<Point4d>> Constellations(ImmutableHashSet<Point4d> points)
        {
            var builder = ImmutableArray.CreateBuilder<ImmutableHashSet<Point4d>>();
            var allNeighbours = GetNeighbours(points);

            var allPoints = new List<Point4d>(points);
            while (allPoints.Count > 0)
            {
                var point = allPoints[0];
                allPoints.RemoveAt(0);
                var constellation = ImmutableHashSet.CreateBuilder<Point4d>();
                constellation.Add(point);

                var neighbours = new Queue<Point4d>(allNeighbours[point]);
                while (neighbours.TryDequeue(out var neighbour))
                {
                    allPoints.Remove(neighbour);
                    constellation.Add(neighbour);

                    foreach (var nn in allNeighbours[neighbour].Where(other => constellation.Contains(other) is false))
                    {
                        neighbours.Enqueue(nn);
                    }
                }

                builder.Add(constellation.ToImmutable());
            }

            return builder.ToImmutable();

            static IReadOnlyDictionary<Point4d, ImmutableHashSet<Point4d>> GetNeighbours(ImmutableHashSet<Point4d> points)
            {
                return points
                    .Select(point => (
                        Source: point,
                        Neighbours: points.Where(p => p != point && PointHelpers.ManhattanDistance(p, point) <= 3).ToImmutableHashSet()
                    ))
                    .ToDictionary(k => k.Source, v => v.Neighbours);
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableHashSet<Point4d> Parse(this IInput input)
        {
            var builder = ImmutableHashSet.CreateBuilder<Point4d>();
            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(Point4d.Parse(line.Span));
            }

            return builder.ToImmutable();
        }
    }
}
