using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using NodaTime;
using Shared;

namespace DayTwenty2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 20), "A Regular Map");

        public override void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();

            var (point, distance) = map
                .MaxBy(x => x.Value)
                .First();

            output.WriteProperty("What is the largest number of doors you would be required to pass through to reach a room?", distance);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var map = input.Parse();

            var numberOfRooms = map.Count(x => x.Value >= 1000);

            output.WriteProperty("How many rooms have a shortest path from your current location that pass through at least 1000 doors?", numberOfRooms);
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableDictionary<Point2d, uint> Parse(this IInput input)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point2d, uint>();
            builder.Add(Point2d.Origin, 0);
            var current = Point2d.Origin;
            var stack = new Stack<Point2d>();

            var span = input.AsString().AsSpan();

            foreach (var c in span)
            {
                switch (c)
                {
                    case '(':
                        stack.Push(current);
                        break;
                    case ')':
                        current = stack.Pop();
                        break;
                    case '|':
                        current = stack.Peek();
                        break;
                    case 'N':
                    case 'S':
                    case 'E':
                    case 'W':
                        var nextDistance = builder[current] + 1;
                        current += c switch
                        {
                            'N' => Direction.North,
                            'E' => Direction.East,
                            'S' => Direction.South,
                            'W' => Direction.West,
                            _ => throw new NotImplementedException(),
                        };
                        builder[current] = Math.Min(builder.GetValueOrDefault(current, uint.MaxValue), nextDistance);
                        break;
                }
            }

            return builder.ToImmutable();
        }
    }
}
