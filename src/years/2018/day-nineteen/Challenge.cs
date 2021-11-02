﻿using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Shared;
using static Shared.DeviceParser;

namespace DayNineteen2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 19), "Go With The Flow");

        public override void PartOne(IInput input, IOutput output)
        {
            var device = ParseDevice(input);
            device.Run();
            var registers = device.Registers;

            output.WriteProperty("Register Zero Value", registers[0]);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            // Found by watching the registers until one is large and stable
            var largeNumber = 10_551_275;

            var factor = Factors(largeNumber).Sum();
            output.WriteProperty("Large Number", largeNumber);
            output.WriteProperty("Factor of Number", factor);

            static IEnumerable<long> Factors(int val) => Enumerable.Range(1, val + 1)
                .Select(i => (long)i)
                .Where(i => val % i == 0);
        }
    }
}
