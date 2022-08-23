using Shared.Grid;

namespace DaySeventeen2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 17), "Trick Shot");

    public void PartOne(IInput input, IOutput output)
    {
        var target = input.Content.ParseArea();

        var velocities = new List<Point2d>();
        for (var x = 0; x < target.MaxX; x++)
        {
            for (var y = target.MinY; y <= -target.MinY; y++)
            {
                velocities.Add((x, y));
            }
        }

        var (path, velocity) = velocities
            .Select(velocity =>
            {
                var path = FireProbe(velocity, target);
                return (Path: path, Velocity: velocity);
            })
            .Where(x => x.Path.Length > 0)
            .MaxBy(x => x.Path.Max(p => p.Y));

        var maxY = path.Max(p => p.Y);

        output.WriteProperty("Velocity", velocity);
        output.WriteProperty("Max Y Position", maxY);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var target = input.Content.ParseArea();

        var velocities = new List<Point2d>();
        for (var x = 0; x <= target.MaxX; x++)
        {
            for (var y = target.MinY; y <= -target.MinY; y++)
            {
                velocities.Add((x, y));
            }
        }

        var count = velocities
            .Select(velocity =>
            {
                var path = FireProbe(velocity, target);
                return (Path: path, Velocity: velocity);
            })
            .Where(x => x.Path.Length > 0)
            .Count();

        output.WriteProperty("Number of velocities", count);
    }

    private static Point2d[] FireProbe(Point2d originalVelocity, Area target)
    {
        var position = Point2d.Origin;
        var velocity = originalVelocity;

        var path = new List<Point2d>()
            {
                position
            };

        while (true)
        {
            position += velocity;
            path.Add(position);

            // If we hit the target then we return the first position we landed in the area.
            if (target.Contains(position))
            {
                return path.ToArray();
            }

            // If we've gone lower than the bottom right we're never coming back around so assume we're never hitting the area
            if (position.Y < target.MinY)
            {
                return Array.Empty<Point2d>();
            }

            var xAdjust = velocity.X switch
            {
                > 0 => -1,
                < 0 => 1,
                0 => 0,
            };
            var adjustment = (xAdjust, -1);
            velocity += adjustment;
        }
    }

}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new("target area: x=(?<x1>-?[0-9]*)..(?<x2>-?[0-9]*), y=(?<y1>-?[0-9]*)..(?<y2>-?[0-9]*)");

    public static Area ParseArea(this IInputContent content)
    {
        var span = content.AsSpan();
        var match = regex.Match(span);

        var x1 = int.Parse(match.Groups["x1"].Value);
        var x2 = int.Parse(match.Groups["x2"].Value);
        var y1 = int.Parse(match.Groups["y1"].Value);
        var y2 = int.Parse(match.Groups["y2"].Value);

        var xMin = Math.Min(x1, x2);
        var xMax = Math.Max(x1, x2);
        var yMin = Math.Min(y1, y2);
        var yMax = Math.Max(y1, y2);

        return new Area(xMin, xMax, yMin, yMax);
    }
}

readonly record struct Area(int MinX, int MaxX, int MinY, int MaxY)
{
    public bool Contains(Point2d point)
    {
        if (point.X < MinX || point.X > MaxX || point.Y < MinY || point.Y > MaxY)
        {
            return false;
        }

        return true;
    }
}
