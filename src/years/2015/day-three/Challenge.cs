using System;
using System.Collections.Generic;
using NodaTime;
using Shared;

namespace DayThree2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 3), "Perfectly Spherical Houses in a Vacuum");

        public override void PartOne(IInput input, IOutput output)
        {
            var position = Point2d.Origin;
            var visisted = new HashSet<Point2d> { position };
            foreach (var command in input.Content.AsSpan())
            {
                var direction = GetDirection(command);
                position += direction;
                visisted.Add(position);
            }

            output.WriteProperty("Visisted Count", visisted.Count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var visisted = new HashSet<Point2d> { Point2d.Origin };
            var pawns = new[] { Point2d.Origin, Point2d.Origin };
            var span = input.Content.AsSpan();
            for (var i = 0; i < span.Length; i++)
            {
                var command = span[i];
                var pawnIndex = i % pawns.Length;
                var pawn = pawns[pawnIndex];

                var direction = GetDirection(command);
                pawns[pawnIndex] = pawn + direction;
                visisted.Add(pawns[pawnIndex]);
            }

            output.WriteProperty("Visisted Count", visisted.Count);
        }

        public static Direction GetDirection(char command) => command switch
        {
            '^' => Direction.North,
            '>' => Direction.East,
            'v' => Direction.South,
            '<' => Direction.West,
            _ => throw new InvalidOperationException("Where?")
        };
    }
}
