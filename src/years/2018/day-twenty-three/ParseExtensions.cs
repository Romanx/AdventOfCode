using System.Text.RegularExpressions;

namespace DayTwentyThree2018
{
    internal static class ParseExtensions
    {
        public static ImmutableArray<Nanobot> Parse(this IInput input)
        {
            var regex = new Regex("^pos=<(?<Position>.*)>, r=(?<Radius>.*)$");
            var builder = ImmutableArray.CreateBuilder<Nanobot>();
            foreach (var line in input.Lines.AsString())
            {
                var result = regex.Match(line);

                var positionArray = result.Groups["Position"].Value
                    .Split(',')
                    .Select(int.Parse)
                    .ToImmutableArray();

                var position = new Point3d(positionArray);
                var radius = uint.Parse(result.Groups["Radius"].Value);

                builder.Add(new(position, radius));
            }

            return builder.ToImmutable();
        }
    }
}
