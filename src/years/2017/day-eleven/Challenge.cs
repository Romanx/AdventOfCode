using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using Shared.HexGrid;

namespace DayEleven2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 11), "Hex Ed");

        public override void PartOne(IInput input, IOutput output)
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

        public override void PartTwo(IInput input, IOutput output)
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
                return str
                    .Split(",", StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries)
                    .Select(s => Parse(s))
                    .ToImmutableArray();
            }
        }

        //   \ n  /
        // nw +--+ ne
        //   /    \
        // -+      +-
        //   \    /
        // sw +--+ se
        //   / s  \
        private static HexDirection Parse(string str)
        {
            return str.Trim().ToUpper() switch
            {
                "N" => HexDirection.NorthWest,
                "NW" => HexDirection.West,
                "NE" => HexDirection.NorthEast,
                "S" => HexDirection.SouthEast,
                "SW" => HexDirection.SouthWest,
                "SE" => HexDirection.East,
                _ => throw new NotImplementedException()
            };
        }
    }
}
