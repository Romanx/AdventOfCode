using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DaySeventeen2020
{
    internal static class ParseExtensions
    {
        public static ImmutableHashSet<Point3d> ParseSpace3d(this IInput input)
        {
            var set = ImmutableHashSet.CreateBuilder<Point3d>();

            var arr = input.Lines.As2DArray();
            for (var row = 0; row < arr.GetLength(0); row++)
            {
                for (var column = 0; column < arr.GetLength(1); column++)
                {
                    if (arr[row, column] == '#')
                    {
                        set.Add(new Point3d(row, column, 0));
                    }
                }
            }

            return set.ToImmutable();
        }

        public static ImmutableHashSet<Point4d> ParseSpace4d(this IInput input) =>
            input.ParseSpace3d()
            .Select(p => new Point4d(Point.ConvertToPoint(p.Dimensions, Point4d.NumberOfDimensions))).ToImmutableHashSet();
    }
}
