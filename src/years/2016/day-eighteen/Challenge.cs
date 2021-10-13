using System;
using System.ComponentModel.DataAnnotations;
using NodaTime;
using Shared;

namespace DayEighteen2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 18), "Like a Rogue");

        public override void PartOne(IInput input, IOutput output)
        {
            var row = input.AsReadOnlyMemory();
            var safeTiles = RoomTrapCalculator.CalculateSafeTiles(row.Span, 40);

            output.WriteProperty("Safe Tiles", safeTiles);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var row = input.AsReadOnlyMemory();
            var safeTiles = RoomTrapCalculator.CalculateSafeTiles(row.Span, 400_000);

            output.WriteProperty("Safe Tiles", safeTiles);
        }
    }

    static class RoomTrapCalculator
    {
        public static uint CalculateSafeTiles(ReadOnlySpan<char> startRow, int rows)
        {
            uint safe = 0;

            var row = Convert(startRow);
            for (var i = 0; i < rows; i++)
            {
                foreach (var state in row)
                {
                    if (state is State.Safe)
                    {
                        safe++;
                    }
                }

                row = NextRow(row);
            }

            return safe;
        }

        private static ReadOnlySpan<State> Convert(ReadOnlySpan<char> row)
        {
            Span<State> next = new State[row.Length];

            for (var i = 0; i < row.Length; i++)
            {
                next[i] = EnumHelpers.FromDisplayName<State>(row[i].ToString());
            }

            return next;
        }

        private static ReadOnlySpan<State> NextRow(ReadOnlySpan<State> previous)
        {
            Span<State> next = new State[previous.Length];

            for (var i = 0; i < previous.Length; i++)
            {
                next[i] = TrapOrNot(i, previous);
            }

            return next;
        }

        private static State TrapOrNot(int index, ReadOnlySpan<State> previous)
        {
            var left = FindState(index - 1, previous);
            var center = FindState(index, previous);
            var right = FindState(index + 1, previous);

            return (left, center, right) switch
            {
                (State.Trap, State.Trap, State.Safe) => State.Trap,
                (State.Safe, State.Trap, State.Trap) => State.Trap,
                (State.Trap, State.Safe, State.Safe) => State.Trap,
                (State.Safe, State.Safe, State.Trap) => State.Trap,
                _ => State.Safe
            };
        }

        private static State FindState(int index, ReadOnlySpan<State> previous)
        {
            if (index >= 0 && index < previous.Length)
            {
                return previous[index];
            }

            return State.Safe;
        }
    }

    enum State
    {
        [Display(Name = "^")]
        Trap,
        [Display(Name = ".")]
        Safe
    }
}
