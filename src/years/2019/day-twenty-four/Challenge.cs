using System.Text;
using CommunityToolkit.HighPerformance;

namespace DayTwentyFour2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 24), "Planet of Discord");

        public void PartOne(IInput input, IOutput output)
        {
            var hashset = new HashSet<string>();

            var bugs = input.ParseBugs();
            hashset.Add(GridPrinter.Print(bugs, '#'));

            while (true)
            {
                bugs = SinglePlanePlanet.Step(bugs);
                var layout = GridPrinter.Print(bugs, '#');

                if (hashset.Add(layout) is false)
                {
                    break;
                }
            }

            var biodiversity = SinglePlanePlanet.CalculateBiodiversity(bugs);

            output.WriteProperty("Biodiversity", biodiversity);
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
        public static ImmutableHashSet<Point2d> ParseBugs(this IInput input)
        {
            var builder = ImmutableHashSet.CreateBuilder<Point2d>();

            var arr = input.Lines.As2DArray().AsSpan2D();
            for (var column = 0; column < arr.Height; column++)
            {
                for (var row = 0; row < arr.Width; row++)
                {
                    if (arr[row, column] == '#')
                    {
                        builder.Add(new Point2d(row, column));
                    }
                }
            }

            return builder.ToImmutable();
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
