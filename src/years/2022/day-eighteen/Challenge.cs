using Shared.Grid;

namespace DayEighteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 18), "Boiling Boulders");

    public void PartOne(IInput input, IOutput output)
    {
        var points = input
            .Lines
            .AsParsable<Point3d>()
            .ToImmutableHashSet();

        var visible = CountVisibleSides(points);

        output.WriteProperty("Visible Sides", visible);

        static long CountVisibleSides(ImmutableHashSet<Point3d> points)
        {
            var sides = 0L;

            foreach (var point in points)
            {
                foreach (var adjacent in Point3d.Adjacent(point))
                {
                    if (points.Contains(adjacent) is false)
                    {
                        sides++;
                    }
                }
            }

            return sides;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var points = input
            .Lines
            .AsParsable<Point3d>()
            .ToImmutableHashSet();

        var area = Area3d.Create(points)
            .Pad(1);

        var currentFrontier = new List<Point3d>
        {
            new Point3d(area.XRange.Min, area.YRange.Min, area.ZRange.Min)
        };

        var nextFrontier = new List<Point3d>();
        var seen = new HashSet<Point3d>();
        var visible = 0L;

        while (currentFrontier.Count > 0)
        {
            foreach (var current in currentFrontier)
            {
                // If we've already seen it then skip it.
                if (seen.Add(current) is false)
                {
                    continue;
                }

                foreach (var next in Point3d.Adjacent(current))
                {
                    // If it's in the area then skip it since we want air
                    // that can reach an outer side
                    if (area.Contains(next))
                    {
                        continue;
                    }

                    // If the adjacent is one of our points then it's side is visible.
                    if (points.Contains(next))
                    {
                        visible++;
                    }
                    else
                    {
                        // If it's not one of our points then it must be air so add it to our search
                        nextFrontier.Add(next);
                    }
                }
            }

            (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
            nextFrontier.Clear();
        }

        output.WriteProperty("Visible Sides", visible);
    }
}
