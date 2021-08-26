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
            var squareFeet = 0;

            foreach (var present in input.Parse())
            {
                squareFeet += present.Surface + present.Sides.Min();
            }

            output.WriteProperty("Total Square Feet", squareFeet);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var ribbon = 0;

            foreach (var present in input.Parse())
            {
                var wrap = 2 * Math.Min(
                    present.Width + present.Length,
                    Math.Min(
                        present.Width + present.Height,
                        present.Length + present.Height
                    )
                );
                var bow = present.Length * present.Width * present.Height;

                ribbon += wrap + bow;
            }

            output.WriteProperty("Total Feet of Ribbon", ribbon);
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Present> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Present>();

            foreach (var line in input.AsStringLines())
            {
                var split = line.Split('x');
                builder.Add(new(
                    int.Parse(split[0]),
                    int.Parse(split[1]),
                    int.Parse(split[2])
                ));
            }

            return builder.ToImmutable();
        }
    }

    record Present
    {
        public Present(int length, int width, int height)
        {
            Length = length;
            Width = width;
            Height = height;
            Sides = ImmutableArray.Create(
                Length * Width,
                Width * Height,
                Height * Length
            );
        }

        public int Length { get; }
        public int Width { get; }
        public int Height { get; }

        public ImmutableArray<int> Sides { get; }

        public int Surface => Sides.Aggregate(0, (acc, i) => acc + i * 2);
    }
}
