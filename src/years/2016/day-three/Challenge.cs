using CommunityToolkit.HighPerformance;

namespace DayThree2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 3), "Squares With Three Sides");

        public void PartOne(IInput input, IOutput output)
        {
            var grid = input.Parse();

            var triangles = AsTriangles(grid);

            output.WriteProperty("Number of valid triangles", triangles
                .Where(t => t.Valid)
                .Count());

            static Triangle[] AsTriangles(int[,] grid)
            {
                var span = grid.AsSpan2D();
                var list = new List<Triangle>(span.Height);

                for (var i = 0; i < span.Height; i++)
                {
                    var row = span.GetRowSpan(i);

                    list.Add(new(
                        row[0],
                        row[1],
                        row[2]));
                }

                return list.ToArray();
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var grid = input.Parse();

            var triangles = AsTriangles(grid);

            output.WriteProperty("Number of valid triangles", triangles
                .Where(t => t.Valid)
                .Count());

            static Triangle[] AsTriangles(int[,] grid)
            {
                var span = grid.AsSpan2D();
                var list = new List<Triangle>();

                for (var x = 0; x < span.Width; x++)
                {
                    for (var y = 0; y < span.Height; y += 3)
                    {
                        var column = span
                            .Slice(y, x, 3, 1)
                            .GetColumn(0)
                            .ToArray();

                        list.Add(new Triangle(
                            column[0],
                            column[1],
                            column[2]));
                    }

                }

                return list.ToArray();
            }
        }
    }
}
