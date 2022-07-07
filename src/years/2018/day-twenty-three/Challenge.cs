using MoreLinq;
using Shared.Graph;

namespace DayTwentyThree2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 23), "Experimental Emergency Teleportation");

        public override void PartOne(IInput input, IOutput output)
        {
            var bots = input.Parse();
            var strongest = bots.OrderByDescending(b => b.Radius).First();

            var count = 0;
            var area = strongest.RadiusArea();

            foreach (var bot in bots)
            {
                if (area.Contains(bot.Position))
                {
                    count++;
                }
            }

            output.WriteProperty("Bots in range", count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var bots = input.Parse();

            var graph = new NanobotGraph();
            foreach (var bot in bots)
            {
                var neighbours = bots
                    .Where(b => b != bot)
                    .Where(b => bot.WithinRangeOfSharedPoint(b));

                graph.AddVertex(bot);
                foreach (var n in neighbours)
                {
                    graph.AddNeighbour(bot, n);
                }
            }

            var clique = graph.BronKerbosch().FindMaximalClique();

            var distanceToPoint = clique.Select(it => it.DistanceTo(Point3d.Origin) - it.Radius).Max();
            output.WriteProperty("Distance to point", distanceToPoint);
        }
    }

    record RadiusArea(Point3d Source, int Radius)
    {
        public bool Contains(Point3d position)
        {
            var manhattan = Manhattan(Source, position);
            return manhattan <= Radius;
        }

        private static int Manhattan(Point3d a, Point3d b) =>
            Math.Abs(a.X - b.X) +
            Math.Abs(a.Y - b.Y) +
            Math.Abs(a.Z - b.Z);
    }

    record Nanobot(Point3d Position, uint Radius)
    {
        public RadiusArea RadiusArea() => new(Position, (int)Radius);

        public bool WithinRangeOfSharedPoint(Nanobot other) =>
            DistanceTo(other.Position) <= Radius + other.Radius;

        public int DistanceTo(Point3d other) =>
            Math.Abs(Position.X - other.X) +
            Math.Abs(Position.Y - other.Y) +
            Math.Abs(Position.Z - other.Z);
    }

    class NanobotGraph : IVertexGraph<Nanobot>
    {
        private readonly Dictionary<Nanobot, ImmutableHashSet<Nanobot>> _graph =
            new();

        public ImmutableArray<Nanobot> Vertexes => _graph.Keys.ToImmutableArray();

        public void AddVertex(Nanobot nanobot)
        {
            _graph.TryAdd(nanobot, ImmutableHashSet<Nanobot>.Empty);
        }

        public void AddNeighbour(Nanobot vertex, Nanobot neighbour)
        {
            AddVertex(vertex);
            AddVertex(neighbour);
            _graph[vertex] = _graph[vertex].Add(neighbour);
            _graph[neighbour] = _graph[neighbour].Add(vertex);
        }

        public IEnumerable<Nanobot> Neigbours(Nanobot node) => _graph[node];
    }
}
