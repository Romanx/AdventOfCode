using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayX2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 0), "");

        public override void PartOne(IInput input, IOutput output)
        {
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }
    }

    internal static class ParseExtensions
    {
    }
}
