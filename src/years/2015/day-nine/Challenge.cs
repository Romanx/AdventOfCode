using System.Text.RegularExpressions;
using static MoreLinq.Extensions.PermutationsExtension;

namespace DayNine2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 9), "All in a Single Night");

        public void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();

            var destinations = map.Keys.ToImmutableArray();

            var (path, weight) = destinations
                .Permutations()
                .Select(perm => CalculateWeight(perm, map))
                .MinBy(x => x.TotalWeight);

            output.WriteProperty("Path", string.Join(" -> ", path));
            output.WriteProperty("Weight", weight);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var map = input.Parse();

            var destinations = map.Keys.ToImmutableArray();

            var (path, weight) = destinations
                .Permutations()
                .Select(perm => CalculateWeight(perm, map))
                .MaxBy(x => x.TotalWeight);

            output.WriteProperty("Path", string.Join(" -> ", path));
            output.WriteProperty("Weight", weight);
        }

        private static (IList<string> Path, int TotalWeight) CalculateWeight(IList<string> path, ImmutableDictionary<string, ImmutableDictionary<string, int>> map)
        {
            var weight = 0;
            var current = path[0];
            for (var i = 1; i < path.Count; i++)
            {
                weight += map[current][path[i]];
                current = path[i];
            }

            return (path, weight);
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex regex = new("(?<Start>.*) to (?<End>.*) = (?<Distance>.*)");

        public static ImmutableDictionary<string, ImmutableDictionary<string, int>> Parse(this IInput input)
        {
            var builder = new Dictionary<string, Dictionary<string, int>>();

            foreach (var line in input.Lines.AsString())
            {
                var match = regex.Match(line);

                var start = match.Groups["Start"].Value;
                var end = match.Groups["End"].Value;
                var dist = int.Parse(match.Groups["Distance"].Value);

                AddEdge(start, end, dist, builder);
            }

            return builder.ToImmutableDictionary(
                k => k.Key,
                v => v.Value.ToImmutableDictionary());

            static void AddEdge(string start, string end, int dist, Dictionary<string, Dictionary<string, int>> builder)
            {
                if (builder.TryGetValue(start, out var existingStart) is false)
                {
                    builder[start] = existingStart = new Dictionary<string, int>();
                }
                existingStart.Add(end, dist);

                if (builder.TryGetValue(end, out var existingEnd) is false)
                {
                    builder[end] = existingEnd = new Dictionary<string, int>();
                }
                existingEnd.Add(start, dist);
            }
        }
    }
}
