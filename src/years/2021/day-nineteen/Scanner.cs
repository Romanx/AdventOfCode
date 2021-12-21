namespace DayNineteen2021;

using System.Diagnostics.CodeAnalysis;
using static MoreLinq.Extensions.SubsetsExtension;

record Scanner
{
    public Scanner(int id, ImmutableHashSet<Point3d> beacons)
    {
        Id = id;
        Beacons = beacons;
        RelationshipsBetweenBeacons = GenerateRelationships(beacons);
    }

    public int Id { get; }

    public ImmutableHashSet<Point3d> Beacons { get; }

    public ImmutableDictionary<int, (Point3d, Point3d)> RelationshipsBetweenBeacons { get; }

    private static ImmutableDictionary<int, (Point3d, Point3d)> GenerateRelationships(ImmutableHashSet<Point3d> beacons)
    {
        var builder = new Dictionary<int, (Point3d, Point3d)>();
        var subsets = beacons.Subsets(2);
        foreach (var subset in subsets)
        {
            var a = subset[0];
            var b = subset[1];
            var distance = a.DistanceBetween(b);

            builder[distance] = (a, b);
        }

        return builder
            .ToImmutableDictionary(k => k.Key, v => v.Value);
    }

    public IEnumerable<(Scanner Scanner, int Configuration)> Rotations()
    {
        for (var configuration = 0; configuration < 24; configuration++)
        {
            yield return RotatedScanner(Id, Beacons, configuration);
        }

        static (Scanner, int) RotatedScanner(int id, ImmutableHashSet<Point3d> beacons, int configuration)
        {
            var builder = ImmutableHashSet.CreateBuilder<Point3d>();
            foreach (var beacon in beacons)
            {
                builder.Add(beacon.ApplyFacingAndRotationChange(configuration));
            }

            return (new Scanner(id, builder.ToImmutable()), configuration);
        }
    }

    public bool Overlaps(Scanner other, [NotNullWhen(true)] out Transform? transform)
    {
        HashSet<int> commonDistances = new(other.RelationshipsBetweenBeacons.Keys);
        commonDistances.IntersectWith(RelationshipsBetweenBeacons.Keys);

        // 12 in common means at least 66 common distances since it's between all the points so
        // 11 + 10 + 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1
        if (commonDistances.Count < 66)
        {
            transform = null;
            return false;
        }

        var testDistance = commonDistances.First();

        var (sourceBeacon, _) = RelationshipsBetweenBeacons[testDistance];

        // For each possible rotation of the other scanner
        foreach (var (rotated, configuration) in other.Rotations())
        {
            var (rotatedA, rotatedB) = rotated.RelationshipsBetweenBeacons[testDistance];

            var offsets = new[]
            {
                sourceBeacon - rotatedA,
                sourceBeacon - rotatedB,
            };

            foreach (var offset in offsets)
            {
                var intersect = Beacons.Intersect(rotated.Beacons.Select(b => b + offset));
                if (intersect.Count >= 12)
                {
                    transform = new Transform(other.Id, offset, configuration);
                    return true;
                }
            }
        }

        transform = null;
        return false;
    }
}
