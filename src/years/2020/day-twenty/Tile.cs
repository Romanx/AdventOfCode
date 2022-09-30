using CommunityToolkit.HighPerformance;

namespace DayTwenty2020
{
    internal record Tile(int Number, char[,] Picture)
    {
        private static readonly ImmutableArray<Func<Tile, Tile>> transforms = ImmutableArray.Create<Func<Tile, Tile>>(
            tile => tile,
            tile => tile.RotateRight(),
            tile => tile.RotateRight(),
            tile => tile.RotateRight(),
            tile => tile.Flip(),
            tile => tile.RotateRight(),
            tile => tile.RotateRight(),
            tile => tile.RotateRight());

        public Tile RotateRight()
        {
            return new Tile(Number, ArrayHelpers.RotateRight(Picture));
        }

        public Tile Flip()
        {
            return new Tile(Number, ArrayHelpers.FlipHorizontal(Picture));
        }

        public IEnumerable<Tile> Orientations()
        {
            var current = this;
            foreach (var transform in transforms)
            {
                current = transform(current);
                yield return current;
            }
        }

        public string Top => new(Picture.GetRowSpan(0));
        public string Left => new(Picture.GetColumn(0).ToArray());
        public string Right => new(Picture.GetColumn(Picture.GetLength(1) - 1).ToArray());
        public string Bottom => new(Picture.GetRowSpan(Picture.GetLength(0) - 1));
    }
}
