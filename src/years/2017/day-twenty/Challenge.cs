using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using NodaTime;
using Shared;
using Shared.Helpers;

namespace DayTwenty2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 20), "Particle Swarm");

        public override void PartOne(IInput input, IOutput output)
        {
            var particles = input.Lines.ParseParticles();

            for (var i = 0; i < 1000; i++)
            {
                particles = Tick(particles);
            }

            var closest = particles
                .MinBy(particle => PointHelpers.ManhattanDistance(particle.Position, Point3d.Origin))
                .First();

            output.WriteProperty("Closest", closest.Id);

            static ImmutableArray<Particle> Tick(ImmutableArray<Particle> particles)
            {
                return particles
                    .Select(static particle => particle.Tick())
                    .ToImmutableArray();
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var particles = input.Lines.ParseParticles();

            for (var i = 0; i < 1000; i++)
            {
                particles = Tick(particles);
            }

            output.WriteProperty("Particles Remaining", particles.Length);

            static ImmutableArray<Particle> Tick(ImmutableArray<Particle> particles)
            {
                return particles
                    .Select(static particle => particle.Tick())
                    .GroupBy(static particle => particle.Position)
                    .Where(group => group.Count() == 1)
                    .SelectMany(group => group)
                    .ToImmutableArray();
            }
        }
    }

    record Particle(int Id, Point3d Position, Point3d Velocity, Point3d Acceleraton)
    {
        public Particle Tick()
        {
            var newVelocity = Velocity + Acceleraton;
            var newPosition = Position + newVelocity;
            return this with
            {
                Position = newPosition,
                Velocity = newVelocity,
            };
        }
    }
}
