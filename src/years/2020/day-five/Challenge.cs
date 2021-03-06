﻿using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using NodaTime;
using Shared;
using Shared.Helpers;

namespace DayFive2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 05), "Binary Boarding");

        public override void PartOne(IInput input, IOutput output)
        {
            var maxSeatId = input.AsLines().Select(line => FindSeatId(line.Span)).Max();

            output.WriteProperty("Seat Id", maxSeatId);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var allSeats = BuildSeatList();
            var occupiedSeats = input.AsLines().Select(line => FindSeatId(line.Span)).ToImmutableHashSet();

            var emptySeats = allSeats.Except(occupiedSeats);

            var seat = emptySeats.First(seat => occupiedSeats.Contains(seat + 1) && occupiedSeats.Contains(seat - 1));

            output.WriteProperty("Seat Id", seat);

            static ImmutableHashSet<int> BuildSeatList()
            {
                var builder = ImmutableHashSet.CreateBuilder<int>();

                for (var column = 0; column < 8; column++)
                {
                    for (var row = 0; row < 128; row++)
                    {
                        builder.Add(row * 8 + column);
                    }
                }

                return builder.ToImmutable();
            }
        }

        private static int FindColumn(ReadOnlySpan<char> span)
        {
            var column = new Range(0, 7);
            foreach (var c in span)
            {
                column = c switch
                {
                    'L' => column.FirstHalf(),
                    'R' => column.LastHalf(),
                    _ => throw new InvalidOperationException("Invalid position")
                };
            }

            return column.Start.Value;
        }

        private static int FindRow(ReadOnlySpan<char> span)
        {
            var row = new Range(0, 127);
            foreach (var c in span)
            {
                row = c switch
                {
                    'F' => row.FirstHalf(),
                    'B' => row.LastHalf(),
                    _ => throw new InvalidOperationException("Invalid position")
                };
            }

            return row.Start.Value;
        }

        private static int FindSeatId(ReadOnlySpan<char> line)
        {
            var row = FindRow(line[..7]);
            var column = FindColumn(line[7..]);

            return row * 8 + column;
        }
    }
}
