using CommunityToolkit.HighPerformance;
using Shared.HexGrid;

namespace DayEleven2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 11), "Hex Ed");

        public void PartOne(IInput input, IOutput output)
        {
            var start = Hex.Origin;
            var directions = input.Content.ParseDirections();

            var end = WalkRoute(start, directions);

            output.WriteProperty("Distance Between", HexHelper.ManhattanDistance(start, end));

            static Hex WalkRoute(Hex start, IEnumerable<HexDirection> directions)
            {
                var current = start;
                foreach (var direction in directions)
                {
                    current += direction;
                }

                return current;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var start = Hex.Origin;
            var directions = input.Content.ParseDirections();

            var (end, max) = WalkRoute(start, directions);

            output.WriteProperty("Distance Between", HexHelper.ManhattanDistance(start, end));
            output.WriteProperty("Furthest Out", max);

            static (Hex, int Max) WalkRoute(Hex start, IEnumerable<HexDirection> directions)
            {
                var max = 0;
                var current = start;
                foreach (var direction in directions)
                {
                    current += direction;
                    max = Math.Max(max, HexHelper.ManhattanDistance(start, current));
                }

                return (current, max);
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<HexDirection> ParseDirections(this IInputContent content)
        {
            return content.Transform(Inner);

            static ImmutableArray<HexDirection> Inner(string str)
            {
                var directions = ImmutableArray.CreateBuilder<HexDirection>();

                foreach (var step in str.Tokenize(','))
                {
                    directions.Add(HexDirection.Parse(step));
                }

                return directions.ToImmutable();
            }
        }
    }
}
