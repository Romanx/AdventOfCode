﻿namespace DaySeventeen2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 17), "Spinlock");

        public void PartOne(IInput input, IOutput output)
        {
            var step = input.Content.AsInt();
            var list = new List<int> { 0 };

            var current = 0;
            for (var i = 1; i <= 2017; i++)
            {
                current = ((current + step) % i) + 1;
                list.Insert(current, i);
            }

            output.WriteProperty("Value after target", list[current + 1 % list.Count]);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var step = input.Content.AsInt();

            var current = 0;
            var one = 0;

            for (var i = 1; i <= 50_000_000; i++)
            {
                current = ((current + step) % i) + 1;
                if (current == 1)
                {
                    one = i;
                }
            }

            output.WriteProperty("Value in Position '1' after 50,000,000 Spins", one);
        }
    }
}
