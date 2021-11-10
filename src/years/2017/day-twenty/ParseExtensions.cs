using MoreLinq;

namespace DayTwenty2017
{
    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"p=<(.+,.*,.*)>, v=<(.*,.*,.*)>, a=<(.*,.*,.*)>");

        public static ImmutableArray<Particle> ParseParticles(this IInputLines lines)
        {
            var builder = ImmutableArray.CreateBuilder<Particle>();
            foreach (var (idx, line) in lines.AsString().Index())
            {
                builder.Add(ParseParticle(idx, line));
            }

            return builder.ToImmutable();

            static Particle ParseParticle(int id, string line)
            {
                var match = regex.Match(line);

                return new Particle(
                    id,
                    ParsePoint(match.Groups[1].Value),
                    ParsePoint(match.Groups[2].Value),
                    ParsePoint(match.Groups[3].Value));
            }

            static Point3d ParsePoint(string input)
            {
                var arr = input.Split(',').Select(int.Parse).ToImmutableArray();
                return new Point3d(arr);
            }
        }
    }
}
