using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Grid
{
    public static class Algorithm
    {
        public static IEnumerable<Point2d> SpiralPointGenerator(
            Point2d start,
            Direction startDirection,
            Rotation rotation)
        {
            var current = start;
            var directions = new Direction[4];
            Func<Direction, Direction> rotationFunc = rotation is Rotation.Clockwise
                ? DirectionActions.TurnRight
                : DirectionActions.TurnLeft;

            directions[0] = startDirection;
            directions[1] = rotationFunc(directions[0]);
            directions[2] = rotationFunc(directions[1]);
            directions[3] = rotationFunc(directions[2]);
            var sideLength = 0;

            yield return current;

            while (true)
            {
                foreach (var direction in directions)
                {
                    if (direction.DirectionType is DirectionType.East or DirectionType.West)
                    {
                        sideLength++;
                    }

                    for (var i = 0; i < sideLength; i++)
                    {
                        current += direction;
                        yield return current;
                    }
                }
            }
        }
    }

    public enum Rotation
    {
        Clockwise,
        CounterClockwise
    }
}
