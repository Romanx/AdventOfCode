using MoreLinq;

namespace DayTwenty2017
{
    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"p=<(.+,.*,.*)>, v=<(.*,.*,.*)>, a=<(.*,.*,.*)>");

        public static ImmutableArray<Particle> ParseParticles(this IInputLines lines)
        {
            var builder = ImmutableArray.CreateBuilder<Particle>();
            foreach (var (idx, line) in lines.AsMemory().Index())
            {
                builder.Add(ParseParticle(idx, line.Span));
            }

            return builder.ToImmutable();

            static Particle ParseParticle(int id, ReadOnlySpan<char> line)
            {
                var match = regex.Match(line);

                return new Particle(
                    id,
                    Point3d.Parse(match.Groups[1].Value),
                    Point3d.Parse(match.Groups[2].Value),
                    Point3d.Parse(match.Groups[3].Value));
            }
        }
    }
}
