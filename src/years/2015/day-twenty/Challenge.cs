using System;
using NodaTime;
using Shared;

namespace DayTwenty2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 20), "Infinite Elves and Infinite Houses");

        private const uint Elves = 1_000_000u;
        private readonly uint[] _houses = new uint[Elves];

        public override void PartOne(IInput input, IOutput output)
        {
            var target = uint.Parse(input.Content.AsString());
            var houses = _houses.AsSpan();
            houses.Clear();

            for (var elf = 1; elf <= Elves; elf++)
            {
                for (var house = elf; house < Elves; house += elf)
                {
                    houses[house - 1] += (uint)(elf * 10u);
                }
            }

            var answer = 0;
            for (var h = 0; h < houses.Length; h++)
            {
                if (houses[h] >= target)
                {
                    answer = h + 1;
                    break;
                }
            }

            output.WriteProperty($"Lowest house number to get '{target}' presents", answer);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var target = uint.Parse(input.Content.AsString());

            var houses = _houses.AsSpan();
            houses.Clear();

            for (var elf = 1; elf <= Elves; elf++)
            {
                for (var house = elf; (house <= elf * 50 && house < Elves); house += elf)
                {
                    houses[house - 1] += (uint)(elf * 11u);
                }
            }

            var answer = 0;
            for (var h = 0; h < houses.Length; h++)
            {
                if (houses[h] >= target)
                {
                    answer = h + 1;
                    break;
                }
            }

            output.WriteProperty($"Lowest house number to get '{target}' presents", answer);
        }
    }
}
