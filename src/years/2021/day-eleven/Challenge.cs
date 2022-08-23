namespace DayEleven2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 11), "Dumbo Octopus");

    public void PartOne(IInput input, IOutput output)
    {
        var total = 0u;
        var octopi = input.ParseOctopi();

        for (var i = 0; i < 100; i++)
        {
            (octopi, var flashed) = Step(octopi);

            total += flashed;
        }

        output.WriteProperty("Number of flashes", total);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var octopi = input.ParseOctopi();

        var turn = 0;

        while (true)
        {
            turn++;
            (octopi, var flashed) = Step(octopi);

            // All of them flashed
            if (flashed == octopi.Count)
            {
                break;
            }
        }

        output.WriteProperty("All flashed on turn", turn);
    }

    private static (ImmutableDictionary<Point2d, uint> Next, uint Flashes) Step(ImmutableDictionary<Point2d, uint> octopi)
    {
        var builder = octopi.ToBuilder();
        foreach (var point in octopi.Keys)
        {
            builder[point]++;
        }

        var flashed = new HashSet<Point2d>();
        var current = new Queue<Point2d>(builder
            .Where(kvp => kvp.Value > 9)
            .Select(kvp => kvp.Key));

        while (current.TryDequeue(out var point))
        {
            if (flashed.Contains(point))
            {
                continue;
            }

            flashed.Add(point);

            var neighbours = Direction.All.Select(dir => point + dir);
            foreach (var neighbour in neighbours)
            {
                if (builder.ContainsKey(neighbour))
                {
                    builder[neighbour]++;

                    if (builder[neighbour] > 9)
                    {
                        current.Enqueue(neighbour);
                    }
                }
            }
        }

        // Set the flashes octopi back to zero.
        foreach (var point in flashed)
        {
            builder[point] = 0;
        }


        return (builder.ToImmutable(), (uint)flashed.Count);
    }
}

internal static class ParseExtensions
{
    public static ImmutableDictionary<Point2d, uint> ParseOctopi(this IInput input)
    {
        return input.As2DPoints()
            .ToImmutableDictionary(k => k.Point, v => (uint)v.Character - '0');
    }
}
