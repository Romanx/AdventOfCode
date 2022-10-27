using Shared.Grid;

namespace DayNineteen2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 19), "A Series of Tubes");

        public void PartOne(IInput input, IOutput output)
        {
            var map = input
                .As2DPoints()
                .Where(c => c.Character is not ' ')
                .ToImmutableDictionary(k => k.Point, v => v.Character);

            var area = Area2d.Create(map.Keys);

            var (point, character) = map
                .Where(k => k.Key.X == 0 || k.Key.Y == 0)
                .Where(v => v.Value is '-' or '|')
                .Single();

            var visited = new HashSet<Point2d>
            {
                point
            };
            var direction = StartingDirection(area, point, character);

            var letters = new List<char>();

            while (true)
            {
                var @char = map[point];

                if (@char is '+')
                {
                    (direction, _) = Directions.CardinalDirections
                        .Select(dir => (Direction: dir, Point: point + dir))
                        .Where(n => map.ContainsKey(n.Point) && visited.Contains(n.Point) is false)
                        .Single();
                }
                else if (char.IsLetter(@char))
                {
                    letters.Add(@char);
                }

                if (map.ContainsKey(point + direction) is false)
                {
                    break;
                }

                point += direction;
                visited.Add(point);
            }

            output.WriteProperty("Final letters", string.Join("", letters));
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var map = input
                .As2DPoints()
                .Where(c => c.Character is not ' ')
                .ToImmutableDictionary(k => k.Point, v => v.Character);

            var area = Area2d.Create(map.Keys);

            var (point, character) = map
                .Where(k => k.Key.X == 0 || k.Key.Y == 0)
                .Where(v => v.Value is '-' or '|')
                .Single();

            var visited = new HashSet<Point2d>
            {
                point
            };
            var direction = StartingDirection(area, point, character);

            var steps = 1;

            while (true)
            {
                var @char = map[point];

                if (@char is '+')
                {
                    (direction, _) = Directions.CardinalDirections
                        .Select(dir => (Direction: dir, Point: point + dir))
                        .Where(n => map.ContainsKey(n.Point) && visited.Contains(n.Point) is false)
                        .Single();
                }

                if (map.ContainsKey(point + direction) is false)
                {
                    break;
                }

                point += direction;
                visited.Add(point);
                steps++;
            }

            output.WriteProperty("Number of steps", steps);
        }

        static Direction StartingDirection(Area2d area, Point2d point, char character)
        {
            return (point, character) switch
            {
                (_, '-') when point.X == 0 => Direction.East,
                (_, '-') when point.X == area.TopRight.X => Direction.West,
                (_, '|') when point.Y == 0 => Direction.South,
                (_, '|') when point.Y == area.BottomLeft.Y => Direction.North,
                _ => throw new NotImplementedException()
            };
        }
    }
}
