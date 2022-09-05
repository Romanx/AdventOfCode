using CommunityToolkit.HighPerformance;
using MoreLinq;

namespace DayTwenty2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 20), "Jurassic Jigsaw");

        public void PartOne(IInput input, IOutput output)
        {
            var tiles = input.ParseTiles();
            var size = (int)Math.Sqrt(tiles.Length);
            var puzzle = BuildPuzzle(tiles, size);

            var corners = new[] {
                puzzle[0, 0],
                puzzle[0, size - 1],
                puzzle[size - 1, 0],
                puzzle[size - 1, size - 1]
            };

            var result = corners.Aggregate(1L, (acc, tile) => acc * tile.Number);

            output.WriteProperty("Total", result);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var tiles = input.ParseTiles();
            var tileCount = (int)Math.Sqrt(tiles.Length);
            var puzzle = BuildPuzzle(tiles, tileCount);

            var image = CombineTiles(puzzle);

            var (monsterCount, monsterSize) = SearchForMonsters(image);

            var seaCount = 0;
            foreach (ref var c in image.AsSpan2D())
            {
                if (c == '#')
                {
                    seaCount++;
                }
            }

            output.WriteProperty("Number of sea tiles", seaCount);
            output.WriteProperty("Number of monsters", monsterCount);
            output.WriteProperty("Result", seaCount - (monsterCount * monsterSize));

            static char[,] CombineTiles(Tile[,] puzzle)
            {
                var tileSize = puzzle[0, 0].Top.Length - 2;
                var tileCount = puzzle.GetLength(0);
                var imageSize = tileSize * tileCount;

                var image = new char[imageSize, imageSize];

                var span = image.AsSpan2D();

                for (var row = 0; row < puzzle.GetLength(0); row++)
                {
                    for (var column = 0; column < puzzle.GetLength(1); column++)
                    {
                        var rowStart = row * tileSize;
                        var colStart = column * tileSize;

                        var dest = span[rowStart..(rowStart + tileSize), colStart..(colStart + tileSize)];

                        var tile = puzzle[row, column];
                        var tilePicture = tile.Picture.AsSpan2D()[1..^1, 1..^1];
                        tilePicture.CopyTo(dest);
                    }
                }

                return image;
            }

            static (int MonsterCount, int MonsterSize) SearchForMonsters(char[,] image)
            {
                var points = GetMonsterPoints();

                var operations = new Func<char[,], char[,]>[]
                {
                    arr => arr,
                    arr => ArrayHelpers.RotateRight(arr),
                    arr => ArrayHelpers.RotateRight(arr),
                    arr => ArrayHelpers.RotateRight(arr),
                    arr => ArrayHelpers.FlipHorizontal(arr),
                    arr => ArrayHelpers.RotateRight(arr),
                    arr => ArrayHelpers.RotateRight(arr),
                    arr => ArrayHelpers.RotateRight(arr),
                };

                var updatedImage = image;
                var monsterCount = 0;
                foreach (var operation in operations)
                {
                    updatedImage = operation(updatedImage);
                    monsterCount += SearchImageForMonsters(points, updatedImage);
                }

                return (monsterCount, points.Length);
            }

            static int SearchImageForMonsters(
                ImmutableArray<Point2d> points,
                char[,] image)
            {
                var count = 0;
                var ranges = PointHelpers.FindSpaceOfPoints(points, Point2d.NumberOfDimensions);
                var xRange = ranges[0];
                var yRange = ranges[1];

                var span = image.AsSpan2D();
                for (var column = 0; column < span.Width; column++)
                {
                    for (var row = 0; row < span.Height; row++)
                    {
                        var xSliceEnd = (column + xRange.Max + 1);
                        var ySliceEnd = (row + yRange.Max + 1);

                        if (xSliceEnd > span.Height || ySliceEnd > span.Width)
                            break;

                        var slice = span[column..xSliceEnd, row..ySliceEnd];

                        var found = true;
                        foreach (var point in points)
                        {
                            if (slice[point.Row, point.Column] != '#')
                            {
                                found = false;
                                break;
                            }
                        }

                        if (found)
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        internal static ImmutableArray<Point2d> GetMonsterPoints()
        {
            var monster = @"                  # 
#    ##    ##    ###
 #  #  #  #  #  #   ";

            return PointHelpers.StringToPoints(monster)
                .Where(p => p.Value == '#')
                .Select(p => p.Key)
                .ToImmutableArray();
        }

        internal static Tile[,] BuildPuzzle(ImmutableArray<Tile> tiles, int size)
        {
            var pairs = new Dictionary<string, List<int>>();
            foreach (var orientedTile in tiles.SelectMany(t => t.Orientations()))
            {
                var pattern = orientedTile.Top;
                if (!pairs.ContainsKey(pattern))
                {
                    pairs[pattern] = new List<int>();
                }

                pairs[pattern].Add(orientedTile.Number);
            }


            var puzzle = new Tile[size, size];
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    var above = row == 0 ? null : puzzle[row - 1, column];
                    var left = column == 0 ? null : puzzle[row, column - 1];
                    puzzle[row, column] = FindTile(above, left, tiles);
                }
            }

            return puzzle;

            bool IsEdge(string pattern) => pairs[pattern].Count == 1;

            int? GetNeighbour(Tile tile, string pattern) => pairs[pattern].SingleOrDefault(other => other != tile.Number);

            Tile FindTile(Tile? above, Tile? left, ImmutableArray<Tile> tiles)
            {
                if (above is null && left is null)
                {
                    // find top-left corner
                    foreach (var tile in tiles)
                    {
                        foreach (var orientedTile in tile.Orientations())
                        {
                            if (IsEdge(orientedTile.Top) && IsEdge(orientedTile.Left))
                            {
                                return orientedTile;
                            }
                        }
                    }
                }
                else
                {
                    var tileNumber = above != null
                        ? GetNeighbour(above, above.Bottom)
                        : GetNeighbour(left!, left!.Right);

                    var tile = tiles.First(t => t.Number == tileNumber);

                    foreach (var orientated in tile.Orientations())
                    {
                        var topMatch = above is null
                            ? IsEdge(orientated.Top)
                            : orientated.Top == above.Bottom;

                        var leftMatch = left is null
                            ? IsEdge(orientated.Left)
                            : orientated.Left == left.Right;

                        if (topMatch && leftMatch)
                        {
                            return orientated;
                        }
                    }
                }

                throw new InvalidOperationException("Could not find tile match");
            }
        }
    }
}
