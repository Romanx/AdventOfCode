﻿using Shared;

namespace DayEleven2020
{
    internal class Map : IEquatable<Map?>
    {
        public ImmutableDictionary<Point2d, bool> Seats { get; }
        public NumberRange<int> Columns { get; }
        public NumberRange<int> Rows { get; }

        public Map(ImmutableDictionary<Point2d, bool> seats, int columns, int rows)
        {
            Seats = seats;
            Columns = new NumberRange<int>(0, columns);
            Rows = new NumberRange<int>(0, rows);
        }

        public Map(ImmutableDictionary<Point2d, bool> seats, NumberRange<int> columns, NumberRange<int> rows)
        {
            Seats = seats;
            Columns = columns;
            Rows = rows;
        }

        public IEnumerable<(Point2d Point, bool Occupied)> AdjacentSeats(Point2d point)
        {
            for (var column = point.Column - 1; column <= point.Column + 1; column++)
            {
                for (var row = point.Row - 1; row <= point.Row + 1; row++)
                {
                    var target = new Point2d(row, column);
                    if (target != point && Seats.TryGetValue(target, out var occupied))
                    {
                        yield return (target, occupied);
                    }
                }
            }
        }

        public IEnumerable<(Point2d Point, bool Occupied)> VisibleAdjacentSeats(Point2d point)
        {
            foreach (var direction in Directions.All)
            {
                var firstInDirection = FirstInDirection(point, direction);
                if (firstInDirection is not null)
                {
                    yield return (firstInDirection.Value, Seats[firstInDirection.Value]);
                }
            }

            Point2d? FirstInDirection(Point2d start, Direction direction)
            {
                var point = start;
                while (Rows.Contains(point.Row) && Columns.Contains(point.Column))
                {
                    var next = point + direction;
                    if (Seats.ContainsKey(next))
                    {
                        return next;
                    }
                    point = next;
                }

                return null;
            }
        }

        public void Print()
        {
            for (var column = 0; column < Columns.End; column++)
            {
                for (var row = 0; row < Rows.End; row++)
                {
                    if (Seats.TryGetValue(new Point2d(row, column), out var occupied))
                    {
                        Console.Write(occupied switch
                        {
                            true => '#',
                            false => 'L'
                        });
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        public override bool Equals(object? obj) => Equals(obj as Map);

        public bool Equals(Map? other)
        {
            return other != null &&
                Seats.Equals(other.Seats);
        }

        public override int GetHashCode() => HashCode.Combine(Seats);
    }
}
