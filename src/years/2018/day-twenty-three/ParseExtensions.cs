using System.Text.RegularExpressions;

namespace DayTwentyThree2018
{
    internal static partial class ParseExtensions
    {
        public static ImmutableArray<Nanobot> Parse(this IInput input)
        {
            var regex = Regex();
            var builder = ImmutableArray.CreateBuilder<Nanobot>();
            foreach (var line in input.Lines.AsString())
            {
                var result = regex.Match(line);
                var position = Point3d.Parse(result.Groups["Position"].Value);
                var radius = uint.Parse(result.Groups["Radius"].Value);

                builder.Add(new(position, radius));
            }

            return builder.ToImmutable();
        }

        [GeneratedRegex("^pos=<(?<Position>.*)>, r=(?<Radius>.*)$")]
        private static partial Regex Regex();
    }
}
