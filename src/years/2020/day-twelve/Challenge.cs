﻿namespace DayTwelve2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 12), "Rain Risk");

        public void PartOne(IInput input, IOutput output)
        {
            var ship = new Ship(Direction.East, Point2d.Origin);

            foreach (var line in input.Lines.AsMemory())
            {
                var span = line.Span;
                var command = span[0];
                var rest = span[1..];

                ship = command switch
                {
                    'N' => ship with { Position = ship.Position + (Direction.North, int.Parse(rest)) },
                    'S' => ship with { Position = ship.Position + (Direction.South, int.Parse(rest)) },
                    'E' => ship with { Position = ship.Position + (Direction.East, int.Parse(rest)) },
                    'W' => ship with { Position = ship.Position + (Direction.West, int.Parse(rest)) },
                    'L' or 'R' => ship with { Facing = MoveByDegrees(ship.Facing, command, int.Parse(rest)) },
                    'F' => ship with
                    {
                        Position = ship.Position + (ship.Facing, int.Parse(rest)),
                    },
                    _ => throw new InvalidOperationException($"Command not recognised {command}"),
                };
            }

            output.WriteProperty("Ship", ship);
            output.WriteProperty("Manhattan Distance", PointHelpers.ManhattanDistance(Point2d.Origin, ship.Position));

            static Direction MoveByDegrees(Direction current, char direction, int value)
            {
                var res = current;
                var count = value / 90;
                for (var i = 0; i < count; i++)
                {
                    if (direction == 'R')
                    {
                        res = res.Right();
                    }
                    else if (direction == 'L')
                    {
                        res = res.Left();
                    }
                }

                return res;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var waypoint = new Point2d(10, -1);
            var ship = new Ship(Direction.East, Point2d.Origin);

            foreach (var line in input.Lines.AsMemory())
            {
                var span = line.Span;
                var command = span[0];
                var argument = int.Parse(span[1..]);

                (ship, waypoint) = command switch
                {
                    'N' => (ship, waypoint + (Direction.North, argument)),
                    'S' => (ship, waypoint + (Direction.South, argument)),
                    'E' => (ship, waypoint + (Direction.East, argument)),
                    'W' => (ship, waypoint + (Direction.West, argument)),
                    'L' or 'R' => (ship, RotateWaypoint(waypoint, command, argument)),
                    'F' => (MoveToNTimes(ship, waypoint, argument), waypoint),
                    _ => throw new InvalidOperationException($"Command not recognised {command}"),
                };
            }

            output.WriteProperty("Ship", ship);
            output.WriteProperty("Manhattan Distance", PointHelpers.ManhattanDistance(Point2d.Origin, ship.Position));

            static Point2d RotateWaypoint(Point2d waypoint, char command, int argument)
            {
                var adjustedAngle = command == 'L'
                    ? -argument
                    : argument;

                return waypoint.RotateAroundPivot(Point2d.Origin, adjustedAngle);
            }

            static Ship MoveToNTimes(Ship ship, Point2d waypoint, int times)
            {
                var pos = ship.Position;
                for (var i = 0; i < times; i++)
                {
                    pos += waypoint;
                }

                return ship with { Position = pos };
            }
        }
    }

    record Ship(Direction Facing, Point2d Position);
}
