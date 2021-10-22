using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime;
using Shared;

namespace DayTwentyFive2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 25), "Let It Snow");

        public override void PartOne(IInput input, IOutput output)
        {
            var (row, column) = input.Parse();

            var result = Enumerable.Range(1, 10000)
                .SelectMany(d => Enumerable.Range(1, d),
                    (d, c) => (Row: d - c + 1, Column: c))
                .TakeWhile(d => d != (row, column))
                .Aggregate(20151125L, (acc, _) => (acc * 252533L) % 33554393L);

            output.WriteProperty($"Result for ({row}, {column})", result);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }
    }

    internal static class ParseExtensions
    {
        public static (int Row, int Column) Parse(this IInput input)
        {
            var regex = new Regex(@"Enter the code at row (?<Row>\d*), column (?<Column>\d*).");
            var match = regex.Match(input.Content.AsString());

            return (
                int.Parse(match.Groups["Row"].Value),
                int.Parse(match.Groups["Column"].Value)
            );
        }
    }
}
