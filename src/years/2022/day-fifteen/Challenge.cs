using System.Diagnostics.CodeAnalysis;
using Shared.Grid;

namespace DayFifteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 15), "Beacon Exclusion Zone");

    public void PartOne(IInput input, IOutput output)
    {
        const int target = 2_000_000;

        var sensors = input.Lines
            .As<Sensor>()
            .ToImmutableArray();

        var hits = new HashSet<int>();

        foreach (var sensor in sensors)
        {
            if (sensor.BoundingBox.YRange.Contains(target))
            {
                // How far from the target row are we
                var distanceToY = int.Abs(sensor.Position.Y - target);

                // How much space distance is left over after we've reached the target row
                var residualLeft = sensor.Distance - distanceToY;

                var segment = LineSegment.Create(
                    (sensor.Position.X - residualLeft, target),
                    (sensor.Position.X + residualLeft, target));

                foreach (var point in segment.Points)
                {
                    if (point == sensor.Position || point == sensor.ClosestBeacon)
                    {
                        continue;
                    }
                    else
                    {
                        hits.Add(point.X);
                    }
                }
            }
        }

        output.WriteProperty("Number of covered spaces", hits.Count);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const long multiplier = 4_000_000;
        const int target = 4_000_000;

        var sensors = input.Lines
            .As<Sensor>()
            .ToImmutableArray();

        var beacon = FindBeacon(sensors);

        output.WriteProperty("Tuning Frequency", (beacon.X * multiplier) + beacon.Y);

        static Point2d FindBeacon(ImmutableArray<Sensor> sensors)
        {
            for (var y = 0; y <= target; y++)
            {
                var x = 0;

                var intersections = sensors
                    .Select(sensor => sensor.IntersectX(y))
                    .Where(range => range.Length > 0)
                    .OrderBy(range => range.Start);

                foreach (var intersection in intersections)
                {
                    // If the intersection starts after this position then we have an empty space.
                    if (intersection.Start > x)
                    {
                        return new Point2d(x, y);
                    }
                    // If the x value is contained inside the intersection then jump to the end of it.
                    else if (x <= intersection.End)
                    {
                        x = intersection.End + 1;
                    }
                }

                // If X still hasn't reached the target then there's a gap
                // until the target where the beacon is.
                if (x < target)
                {
                    return (x, y);
                }
            }

            throw new InvalidOperationException("Nope didn't find the beacon");
        }
    }
}

internal static class ParseExtensions
{
}

readonly record struct Sensor(Point2d Position, Point2d ClosestBeacon)
    : ISpanParsable<Sensor>
{
    private static readonly PcreRegex sensorRegex = new(@"Sensor at x=(?<SensorX>[+-]?\d+), y=(?<SensorY>[+-]?\d+): closest beacon is at x=(?<BeaconX>[+-]?\d+), y=(?<BeaconY>[+-]?\d+)");

    public int Distance { get; } = Point2d.ManhattanDistance(Position, ClosestBeacon);

    public Area2d BoundingBox { get; } = CreateBoundingBox(Position, Point2d.ManhattanDistance(Position, ClosestBeacon));

    public bool WithinRange(Point2d point)
        => Point2d.ManhattanDistance(Position, point) <= Distance;

    public NumberRange<int> IntersectX(int target)
    {
        // How far from the target row are we
        var distanceToY = int.Abs(Position.Y - target);

        if (distanceToY > Distance)
        {
            return NumberRange<int>.Empty;
        }

        // How much space distance is left over after we've reached the target row
        var residualLeft = Distance - distanceToY;

        return new NumberRange<int>(Position.X - residualLeft, Position.X + residualLeft);
    }

    private static Area2d CreateBoundingBox(Point2d position, int distance)
    {
        var topLeft = new Point2d(position.X - distance, position.Y - distance);
        var bottomRight = new Point2d(position.X + distance, position.Y + distance);

        return Area2d.Create(topLeft, bottomRight);
    }

    public static Sensor Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var sensor))
        {
            return sensor;
        }

        throw new InvalidOperationException("Unable to parse sensor");
    }

    public static Sensor Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Sensor result)
    {
        var match = sensorRegex.Match(s);

        if (match.Success)
        {
            var position = new Point2d(
                int.Parse(match.Groups["SensorX"].Value),
                int.Parse(match.Groups["SensorY"].Value));

            var beacon = new Point2d(
                int.Parse(match.Groups["BeaconX"].Value),
                int.Parse(match.Groups["BeaconY"].Value));

            
            result = new Sensor(position, beacon);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Sensor result)
        => TryParse(s.AsSpan(), provider, out result);
}
