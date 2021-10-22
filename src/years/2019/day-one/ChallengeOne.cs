using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Shared;

namespace DayOne2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 01), "The Tyranny of the Rocket Equation");

        public override void PartOne(IInput input, IOutput output)
        {
            var totalMass = input
                .AsInts()
                .Select(CalculateFuel)
                .Sum();

            output.WriteProperty("Total Mass", $"{totalMass:N0}");
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var totalMass = input
                .AsInts()
                .Select(AggregateFuel)
                .Sum();

            output.WriteProperty("Total Mass", $"{totalMass:N0}");
        }

        private int CalculateFuel(int mass) => ((int)Math.Floor(mass / 3m)) - 2;

        private long AggregateFuel(int mass)
        {
            var total = CalculateFuel(mass);
            return total <= 0
                ? 0L
                : total + AggregateFuel(total);
        }
    }

    internal static class InputExtensions
    {
        public static IEnumerable<int> AsInts(this IInput input) => input.Lines.AsMemory().Select(mem => int.Parse(mem.Span));
    }
}
