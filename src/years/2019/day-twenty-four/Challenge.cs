using System.Text;
using CommunityToolkit.HighPerformance;
using Shared.Grid;

namespace DayTwentyFour2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 24), "Planet of Discord");

        public void PartOne(IInput input, IOutput output)
        {
            var hashset = new HashSet<Plane>();

            var plane = input.ParseBugs();

            while (true)
            {
                plane = SinglePlanePlanet.Step(plane);

                if (hashset.Add(plane) is false)
                {
                    break;
                }
            }

            output.WriteProperty("Biodiversity", plane.Biodiversity);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var bugs = input.ParseBugs3d();

            var result = MultiPlanePlanet.Run(200, bugs);

            MultiPlanePlanet.Print(result);
            output.WriteProperty("Number of Bugs", result.Count);
        }
    }

    internal static class ParseExtensions
    {
        public static Plane ParseBugs(this IInput input)
        {
            var plane = new Plane(Area2d.Create(5, 5));

            var points = input.As2DPoints()
                .Where(kvp => kvp.Character is '#')
                .Select(kvp => kvp.Point);

            plane.SetAll(points, true);

            return plane;
        }

        public static ImmutableHashSet<Point3d> ParseBugs3d(this IInput input)
        {
            var builder = ImmutableHashSet.CreateBuilder<Point3d>();

            var arr = input.Lines.As2DArray().AsSpan2D();
            for (var column = 0; column < arr.Height; column++)
            {
                for (var row = 0; row < arr.Width; row++)
                {
                    if (arr[row, column] == '#')
                    {
                        builder.Add(new Point3d(row, column, 0));
                    }
                }
            }

            return builder.ToImmutable();
        }
    }
}
