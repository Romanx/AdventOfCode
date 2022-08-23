using static MoreLinq.Extensions.SubsetsExtension;

namespace DayNineteen2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 19), "Beacon Scanner");

    public void PartOne(IInput input, IOutput output)
    {
        var scanners = input.Parse();

        var (combinedScanner, _) = ReorientScannerRegions(scanners);

        output.WriteProperty("Number of Beacons in total", combinedScanner.Beacons.Count);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var scanners = input.Parse();

        var (_, scannerPositions) = ReorientScannerRegions(scanners);

        var maximumDistance = scannerPositions
            .Subsets(2)
            .Select(subset => PointHelpers.ManhattanDistance(subset[0], subset[1]))
            .Max();

        output.WriteProperty("Max Distance between Scanners", maximumDistance);
    }

    private static (Scanner CombinedScanner, ImmutableArray<Point3d> Scanners) ReorientScannerRegions(ImmutableArray<Scanner> scanners)
    {
        var source = scanners[0];
        var queue = new Queue<Scanner>(scanners.Skip(1));
        var foundScanners = ImmutableArray.CreateBuilder<Point3d>();
        foundScanners.Add(Point3d.Origin);

        while (queue.TryDequeue(out var scanner))
        {
            if (source.Overlaps(scanner, out var transform))
            {
                var additionalBeacons = source.Beacons
                    .Union(transform.Apply(scanner.Beacons));

                foundScanners.Add(transform.Origin);

                source = new Scanner(source.Id, additionalBeacons);
            }
            else
            {
                queue.Enqueue(scanner);
            }
        }

        return (source, foundScanners.ToImmutable());
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Scanner> Parse(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var builder = ImmutableArray.CreateBuilder<Scanner>();

        for (var i = 0; i < paragraphs.Length; i++)
        {
            var paragraph = paragraphs[i];
            var points = paragraph[1..];
            var set = ImmutableHashSet.CreateBuilder<Point3d>();
            foreach (var point in points.Span)
            {
                set.Add(Point3d.Parse(point.Span));
            }
            builder.Add(new Scanner(i, set.ToImmutable()));
        }

        return builder.ToImmutable();
    }
}

internal static class PointExtensions
{
    public static Point3d ApplyFacingAndRotationChange(this Point3d input, int configuration)
    {
        var x = input.X;
        var y = input.Y;
        var z = input.Z;

        return configuration switch
        {
            0 => new(x, y, z),
            1 => new(-y, x, z),
            2 => new(-x, -y, z),
            3 => new(y, -x, z),
            4 => new(y, -z, -x),
            5 => new(z, y, -x),
            6 => new(-y, z, -x),
            7 => new(-z, -y, -x),
            8 => new(-z, x, -y),
            9 => new(-x, -z, -y),
            10 => new(z, -x, -y),
            11 => new(x, z, -y),
            12 => new(y, z, x),
            13 => new(-z, y, x),
            14 => new(-y, -z, x),
            15 => new(z, -y, x),
            16 => new(-y, -x, -z),
            17 => new(x, -y, -z),
            18 => new(y, x, -z),
            19 => new(-x, y, -z),
            20 => new(-x, z, y),
            21 => new(-z, -x, y),
            22 => new(x, -z, y),
            23 => new(z, x, y),
            _ => throw new InvalidOperationException("Invalid Configuration!")
        };
    }

    ///https://brilliant.org/wiki/3d-coordinate-geometry-distance/
    public static int DistanceBetween(this Point3d input, Point3d other)
    {
        var x = other.X - input.X;
        var y = other.Y - input.Y;
        var z = other.Z - input.Z;

        return (x * x) + (y * y) + (z * z);

        //return (int)Math.Sqrt((x * x) + (y * y) + (z * z));
    }
}
