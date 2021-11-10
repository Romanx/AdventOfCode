namespace DayTwentyFour2020
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 24), "Lobby Layout");

        public override void PartOne(IInput input, IOutput output)
        {
            var paths = input.ParsePaths();

            var blackTiles = FindBlackTiles(paths);

            output.WriteProperty("Number of black tiles", blackTiles.Count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var paths = input.ParsePaths();
            var blackTiles = FindBlackTiles(paths);

            for (var day = 1; day <= 100; day++)
            {
                blackTiles = CalculateDayLayout(blackTiles);
            }

            output.WriteProperty("Number of black tiles", blackTiles.Count);

            static ImmutableHashSet<Point3d> CalculateDayLayout(ImmutableHashSet<Point3d> blackTiles)
            {
                var result = blackTiles.ToBuilder();

                var candidates = blackTiles
                    .SelectMany(t => AdjacentHexPoints(t).Append(t))
                    .Distinct();

                foreach (var candidiate in candidates)
                {
                    var adjacentBlackTiles = AdjacentHexPoints(candidiate).Count(p => blackTiles.Contains(p));

                    if (blackTiles.Contains(candidiate))
                    {
                        if (adjacentBlackTiles is 0 or > 2)
                        {
                            result.Remove(candidiate);
                        }
                    }
                    else
                    {
                        if (adjacentBlackTiles is 2)
                        {
                            result.Add(candidiate);
                        }
                    }
                }

                return result.ToImmutable();
            }
        }

        private static ImmutableHashSet<Point3d> FindBlackTiles(ImmutableArray<ImmutableArray<HexDirection>> paths)
        {
            var blackTiles = ImmutableHashSet<Point3d>.Empty;

            foreach (var path in paths)
            {
                blackTiles = WalkPath(path, blackTiles);
            }

            return blackTiles;

            static ImmutableHashSet<Point3d> WalkPath(ImmutableArray<HexDirection> path, ImmutableHashSet<Point3d> blackTiles)
            {
                var point = Point3d.Origin;

                foreach (var step in path)
                {
                    point = step switch
                    {
                        HexDirection.East => point + (1, -1, 0),
                        HexDirection.SouthEast => point + (0, -1, 1),
                        HexDirection.SouthWest => point + (-1, 0, 1),
                        HexDirection.West => point + (-1, 1, 0),
                        HexDirection.NorthWest => point + (0, 1, -1),
                        HexDirection.NorthEast => point + (1, 0, -1),
                        _ => throw new NotImplementedException(),
                    };
                }

                return blackTiles.Contains(point)
                    ? blackTiles.Remove(point)
                    : blackTiles.Add(point);
            }
        }

        private static IEnumerable<Point3d> AdjacentHexPoints(Point3d point)
        {
            yield return point + (1, -1, 0);
            yield return point + (0, -1, 1);
            yield return point + (-1, 0, 1);
            yield return point + (-1, 1, 0);
            yield return point + (0, 1, -1);
            yield return point + (1, 0, -1);
        }
    }

    enum HexDirection
    {
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
        NorthEast
    }
}
