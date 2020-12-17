using System;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayEleven2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 11), "Seating System");

        public override void PartOne(IInput input, IOutput output)
        {
            Map map = input.ParseMap();
            var result = RunUntilMapStable(map, AdjacentFunction);

            var numberOfOccupiedSeats = result.Seats.Count(s => s.Value is true);
            output.WriteProperty("Number of occupied seats", numberOfOccupiedSeats);

            static bool AdjacentFunction(Map map, Point2d seat, bool occupied)
            {
                var occupiedCount = map.AdjacentSeats(seat).Count(p => p.Occupied is true);

                return occupied ? occupiedCount < 4 : occupiedCount == 0;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            Map map = input.ParseMap();
            var result = RunUntilMapStable(map, AdjacentFunction);

            var numberOfOccupiedSeats = result.Seats.Count(s => s.Value is true);

            output.WriteProperty("Number of occupied seats", numberOfOccupiedSeats);

            static bool AdjacentFunction(Map map, Point2d seat, bool occupied)
            {
                var occupiedCount = map.VisibleAdjacentSeats(seat).Count(p => p.Occupied is true);

                return occupied ? occupiedCount < 5 : occupiedCount == 0;
            }
        }

        internal static Map RunUntilMapStable(Map current, Func<Map, Point2d, bool, bool> adjacentFunc)
        {
            Map next = current;

            do
            {
                current = next;
                next = Step(current);
            } while (next.Equals(current) is false);

            return next;

            Map Step(Map input)
            {
                var result = input.Seats.ToBuilder();

                foreach (var (seat, occupied) in input.Seats)
                {
                    result[seat] = adjacentFunc(input, seat, occupied);
                }

                return new Map(result.ToImmutable(), input.Columns, input.Rows);
            }
        }
    }

    internal static class ParseExtensions
    {
        public static Map ParseMap(this IInput input)
        {
            var seats = ImmutableDictionary.CreateBuilder<Point2d, bool>();

            var arr = input.As2DArray();
            var rows = arr.GetLength(0);
            var columns = arr.GetLength(1);

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (arr[row, column] is 'L' or '#')
                    {
                        seats.Add(new(row, column), arr[row, column] == '#');
                    }
                }
            }

            return new Map(seats.ToImmutable(), columns, rows);
        }
    }
}
