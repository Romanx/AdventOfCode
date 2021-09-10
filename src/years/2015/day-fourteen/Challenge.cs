using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using MoreLinq.Extensions;
using NodaTime;
using Shared;

namespace DayFourteen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 14), "Reindeer Olympics");

        public override void PartOne(IInput input, IOutput output)
        {
            var reindeers = input.Parse();
            const int Timestamp = 2503;

            var fastestReindeer = reindeers
                .MaxBy(r => DistanceAfterTime(r, Timestamp))
                .First();

            output.WriteProperty("Fastest Reindeer", fastestReindeer.Name);
            output.WriteProperty("Distance after time", DistanceAfterTime(fastestReindeer, Timestamp));

            static int DistanceAfterTime(Reindeer reindeer, int time)
            {
                var cycleTime = reindeer.CycleTime;
                var cycles = time / cycleTime;

                var total = cycles * reindeer.DistancePerCycle;
                var current = cycles * cycleTime;

                for (var i = current; i < time; i++)
                {
                    var offset = i % cycleTime;
                    if (offset < reindeer.FlyTime)
                    {
                        total += reindeer.Speed;
                    }
                }

                return total;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var reindeers = input.Parse();
            const int Timestamp = 2503;
            var points = reindeers.ToDictionary(k => k, _ => 0);
            var distance = reindeers.ToDictionary(k => k, _ => 0);

            for (var i = 0; i < Timestamp; i++)
            {
                foreach (var reindeer in reindeers)
                {
                    var offset = i % reindeer.CycleTime;
                    if (offset < reindeer.FlyTime)
                    {
                        distance[reindeer] += reindeer.Speed;
                    }
                }

                var furthest = distance.MaxBy(v => v.Value);
                foreach (var (reindeer, _) in furthest)
                {
                    points[reindeer]++;
                }
            }

            var (fastestReindeer, highestPoints) = points
                .MaxBy(r => r.Value)
                .First();

            output.WriteProperty("Fastest Reindeer", fastestReindeer.Name);
            output.WriteProperty("Points after time", highestPoints);
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex regex = new(@"(?<Reindeer>.*) can fly (?<Speed>[0-9]*) km/s for (?<FlyTime>[0-9]*) seconds, but then must rest for (?<RestTime>[0-9]*) seconds.");

        public static ImmutableArray<Reindeer> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Reindeer>();

            foreach (var line in input.AsStringLines())
            {
                var match = regex.Match(line);

                builder.Add(new(
                    match.Groups["Reindeer"].Value,
                    int.Parse(match.Groups["Speed"].Value),
                    int.Parse(match.Groups["FlyTime"].Value),
                    int.Parse(match.Groups["RestTime"].Value)));
            }

            return builder.ToImmutable();
        }
    }

    record Reindeer(string Name, int Speed, int FlyTime, int RestTime)
    {
        public int DistancePerCycle { get; } = Speed * FlyTime;

        public int CycleTime { get; } = FlyTime + RestTime;
    }
}
