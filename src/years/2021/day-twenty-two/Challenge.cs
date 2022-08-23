using Shared.Grid;

namespace DayTwentyTwo2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 22), "Reactor Reboot");

    public void PartOne(IInput input, IOutput output)
    {
        var targetRegion = new Area3d(
            new DimensionRange(-50, 50),
            new DimensionRange(-50, 50),
            new DimensionRange(-50, 50));

        var cubes = input.Lines.ParseCubeRegions();

        var validCubes = cubes
            .Where(cube => cube.Area.Intersects(targetRegion));

        var litCubes = NumberOfLitCubes(validCubes);

        output.WriteProperty("Number of lit cubes", litCubes);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var cubes = input.Lines.ParseCubeRegions();
        var litCubes = NumberOfLitCubes(cubes);

        output.WriteProperty("Number of lit cubes", litCubes);
    }

    static long NumberOfLitCubes(IEnumerable<PowerCubeRegion> cubes)
    {
        var volumes = new List<PowerCubeRegion>();

        foreach (var cube in cubes)
        {
            volumes.AddRange(FindOverlappingRegions(cube, volumes));

            if (cube.Lit)
            {
                volumes.Add(cube);
            }
        }

        return volumes
            .Sum(cube => cube.Area.NumberOfPoints * (cube.Lit ? 1 : -1));

        static List<PowerCubeRegion> FindOverlappingRegions(PowerCubeRegion cube, List<PowerCubeRegion> volumes)
        {
            List<PowerCubeRegion> overlapping = new();
            foreach (var known in volumes)
            {
                var intersect = known.Area.Intersect(cube.Area);

                if (intersect is not null)
                {
                    // If we've intersected then we're adding the inverse of the intersected state
                    overlapping.Add(new PowerCubeRegion(!known.Lit, intersect.Value));
                }
            }

            return overlapping;
        }
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex areaRegex = new(@"(?<Lit>on|off) x=(?<xMin>-?\d+)..(?<xMax>-?\d+),y=(?<yMin>-?\d+)..(?<yMax>-?\d+),z=(?<zMin>-?\d+)..(?<zMax>-?\d+)");

    public static ImmutableArray<PowerCubeRegion> ParseCubeRegions(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<PowerCubeRegion>();

        foreach (var memory in lines.AsMemory())
        {
            var match = areaRegex.Match(memory.Span);

            var lit = match.Groups["Lit"].Value.SequenceEqual("on");
            var area = ParseArea(match.Groups);

            builder.Add(new PowerCubeRegion(lit, area));
        }

        return builder.ToImmutable();

        static Area3d ParseArea(PcreRefMatch.GroupList groups)
        {
            return new Area3d(
                new DimensionRange(int.Parse(groups["xMin"].Value), int.Parse(groups["xMax"].Value)),
                new DimensionRange(int.Parse(groups["yMin"].Value), int.Parse(groups["yMax"].Value)),
                new DimensionRange(int.Parse(groups["zMin"].Value), int.Parse(groups["zMax"].Value))
            );
        }
    }
}

readonly record struct PowerCubeRegion(bool Lit, Area3d Area);
