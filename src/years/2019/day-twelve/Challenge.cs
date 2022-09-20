namespace DayTwelve2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 12), "The N-Body Problem");

    public void PartOne(IInput input, IOutput output)
    {
        var moons = input.Parse();

        var result = Simulator.SimulateUntilStep(moons, step: 1000);

        var totalPotentialEnergy = result
            .Sum(x => CalculatePotentialEnergy(x.Position, x.Velocity));

        output.WriteProperty("Total Potential Energy", totalPotentialEnergy);

        static int CalculatePotentialEnergy(Point3d position, Point3d velocity)
        {
            var potential = Math.Abs(position.X) + Math.Abs(position.Y) + Math.Abs(position.Z);
            var kinetic = Math.Abs(velocity.X) + Math.Abs(velocity.Y) + Math.Abs(velocity.Z);

            return potential * kinetic;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var moons = input.Parse();

        var step = Simulator.SimulateUntilLoop(moons);

        output.WriteProperty("Step that loop occurs", step);
    }
}

public static class Simulator
{
    public static ImmutableArray<Moon> SimulateUntilStep(
        ImmutableArray<Moon> moons,
        int step)
    {
        var current = moons.ToArray();
        var next = new Moon[moons.Length];

        for (var iteration = 1; iteration <= step; iteration++)
        {
            PerformStep(ref current, ref next);
        }

        return current.ToImmutableArray();
    }

    public static long SimulateUntilLoop(ImmutableArray<Moon> moons)
    {
        var current = moons.ToArray();
        var next = new Moon[moons.Length];

        var cycle = new long[3];
        Array.Fill(cycle, 0);

        var shouldContinue = false;
        long step = 0;
        do
        {
            PerformStep(ref current, ref next);
            step++;
            shouldContinue = false;
            for (var i = 0; i < 3; i++)
            {
                if (cycle[i] != 0) continue;
                shouldContinue = true;

                var isMatch = current
                    .Select((item, idx) =>
                    {
                        var start = moons[idx];

                        return MatchPositionAtIndexAndZeroVelocity(i, start.Position, item);
                    })
                    .All(x => x);

                if (isMatch)
                {
                    cycle[i] = step;
                }
            }

        } while (shouldContinue);

        return cycle.Aggregate(1L, MathHelper.LowestCommonMultiple);

        static bool MatchPositionAtIndexAndZeroVelocity(int index, Point3d startPosition, Moon current)
        {
            return startPosition[index] == current.Position[index] && current.Velocity[index] == 0;
        }
    }

    private static void PerformStep(ref Moon[] current, ref Moon[] next)
    {
        for (var i = 0; i < current.Length; i++)
        {
            var item = current[i];

            var (x, y, z) = item.Velocity;
            for (var otherIndex = 0; otherIndex < current.Length; otherIndex++)
            {
                if (otherIndex == i) continue;
                var other = current[otherIndex];

                x += other.Position.X.CompareTo(item.Position.X);
                y += other.Position.Y.CompareTo(item.Position.Y);
                z += other.Position.Z.CompareTo(item.Position.Z);
            }

            Point3d newVelocity = (x, y, z);
            next[i] = new Moon(
                item.Position + newVelocity,
                newVelocity);
        }

        (current, next) = (next, current);
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex _regex = new(@"<x=(?<x>-?\d+), y=(?<y>-?\d+), z=(?<z>-?\d+)>");

    public static ImmutableArray<Moon> Parse(this IInput input) => input
        .Lines
        .Transform(static line =>
        {
            var match = _regex.Match(line.AsSpan());

            var position = new Point3d(
                int.Parse(match.Groups["x"].Value),
                int.Parse(match.Groups["y"].Value),
                int.Parse(match.Groups["z"].Value)
            );

            return new Moon(position, Point3d.Origin);
        })
        .ToImmutableArray();
}

public readonly record struct Moon(Point3d Position, Point3d Velocity);
