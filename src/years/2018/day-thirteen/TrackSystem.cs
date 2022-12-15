namespace DayThirteen2018;

class TrackSystem
{
    private readonly ImmutableDictionary<Point2d, TrackType> _grid;

    public TrackSystem(ImmutableDictionary<Point2d, TrackType> grid, ImmutableArray<Minecart> minecarts)
    {
        _grid = grid;
        Minecarts = minecarts;
    }

    public TrackType this[Point2d position] => _grid[position];
    public ImmutableArray<Minecart> Minecarts { get; }

    public IEnumerable<Point2d> Collisions()
    {
        while (Minecarts.Count(cart => cart.Alive) > 1)
        {
            var livingCarts = Minecarts
                .Where(cart => cart.Alive)
                .OrderBy(cart => cart.Position, ReadingOrderComparer.Instance);

            foreach (var minecart in livingCarts)
            {
                minecart.Move(this);

                foreach (var other in Minecarts)
                {
                    if (other.CollidesWith(minecart))
                    {
                        minecart.Alive = false;
                        other.Alive = false;
                        yield return minecart.Position;
                    }
                }
            }
        }
    }

    public string Print()
    {
        return GridPrinter.Print(_grid, '#', (map, writer, empty) =>
        {
            return new TrackPrinter(map, Minecarts, writer);
        });
    }

    private class TrackPrinter : GridPrinter<TrackType>
    {
        private readonly ImmutableDictionary<Point2d, Minecart> _minecarts;

        public TrackPrinter(
            IReadOnlyDictionary<Point2d, TrackType> map,
            ImmutableArray<Minecart> minecarts,
            IGridWriter writer)
            : base(map, writer, '#')
        {
            _minecarts = minecarts.ToImmutableDictionary(k => k.Position, v => v);
        }

        public override void OnPosition(Point2d point)
        {
            if (_minecarts.TryGetValue(point, out var minecart))
            {
                var character = minecart.Facing.DirectionType switch
                {
                    DirectionType.North => "^",
                    DirectionType.East => ">",
                    DirectionType.South => "v",
                    DirectionType.West => "<",
                    _ => throw new InvalidOperationException("How did it end up that direction!"),
                };

                _writer.Append(character);
            }
            else if (_map.TryGetValue(point, out var type))
            {
                _writer.Append(EnumHelpers.ToDisplayName(type));
            }
        }
    }
}
