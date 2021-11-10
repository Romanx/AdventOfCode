using System.Text.RegularExpressions;

namespace DaySeventeen2018
{
    internal static class ParseExtensions
    {
        private static readonly Regex lineRegex = new(@"(?<DimensionA>.)=(?<DimensionAVal>.*), (?<DimensionB>.)=(?<DimensionBStart>.*)\.\.(?<DimensionBEnd>.*)");

        public static ImmutableHashSet<Point2d> ParseClayPoints(this IInput input)
        {
            var builder = ImmutableHashSet.CreateBuilder<Point2d>();
            foreach (var line in input.Lines.AsString())
            {
                builder.UnionWith(ParseLine(line));
            }

            return builder.ToImmutable();

            static IEnumerable<Point2d> ParseLine(string line)
            {
                var matches = lineRegex.Match(line);

                var dimensionA = matches.Groups["DimensionA"].Value[0];
                var dimensionAValue = int.Parse(matches.Groups["DimensionAVal"].Value);

                var dimensionB = matches.Groups["DimensionB"].Value[0];
                var startRange = int.Parse(matches.Groups["DimensionBStart"].Value);
                var endRange = int.Parse(matches.Groups["DimensionBEnd"].Value);
                var range = Enumerable.Range(startRange, (endRange - startRange) + 1);

                foreach (var idx in range)
                {
                    yield return dimensionA switch
                    {
                        'x' => new Point2d(dimensionAValue, idx),
                        'y' => new Point2d(idx, dimensionAValue),
                        _ => throw new NotImplementedException(),
                    };
                }
            }
        }
    }
}
