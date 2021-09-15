using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DayTwentyOne2015
{
    internal static class ParseExtensions
    {
        public static Unit Parse(this IInput input)
        {
            var lines = input.AsStringLines().ToArray();

            return new Unit(
                UnitType.Boss,
                int.Parse(lines[0].Split(':')[1]),
                int.Parse(lines[1].Split(':')[1]),
                int.Parse(lines[2].Split(':')[1]),
                ImmutableArray<Equipment>.Empty);
        }
    }
}
