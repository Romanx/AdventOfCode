using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayTwo2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 2), "I Was Told There Would Be No Math");

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