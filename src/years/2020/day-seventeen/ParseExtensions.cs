namespace DaySeventeen2020
{
    internal static class ParseExtensions
    {
        public static ImmutableHashSet<Point3d> ParseSpace3d(this IInput input) => ParseInputPoints(input)
            .Select(point => point.Z(0))
            .ToImmutableHashSet();

        public static ImmutableHashSet<Point4d> ParseSpace4d(this IInput input) => ParseInputPoints(input)
            .Select(point => new Point4d(point.X, point.Y, 0, 0))
            .ToImmutableHashSet();

        private static IEnumerable<Point2d> ParseInputPoints(IInput input) =>
            input
                .As2DPoints()
                .Where(kvp => kvp.Character is '#')
                .Select(kvp => kvp.Point);
    }
}
