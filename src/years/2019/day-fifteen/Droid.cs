using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace DayFifteen2019;

internal class Droid
{
    private readonly IntcodeComputer computer;

    public Droid(ImmutableArray<long> memory)
    {
        computer = new IntcodeComputer(memory);
    }

    public async Task<ImmutableDictionary<Point2d, CellType>> Run()
    {
        var map = new Dictionary<Point2d, CellType>()
        {
            [Point2d.Origin] = CellType.Empty,
        };
        var cts = new CancellationTokenSource();
        var task = computer.Run(cts.Token);

        await Discover(Point2d.Origin, map);
        cts.Cancel();
        await task;
        return map.ToImmutableDictionary();
    }

    private async Task Discover(Point2d point, Dictionary<Point2d, CellType> map)
    {
        await CheckDirection(Direction.North);
        await CheckDirection(Direction.East);
        await CheckDirection(Direction.South);
        await CheckDirection(Direction.West);

        async Task CheckDirection(Direction direction)
        {
            var newLocation = point + direction;
            if (map.ContainsKey(newLocation))
            {
                return;
            }

            await MoveDirection(newLocation, direction, map);
        }
    }

    private async Task MoveDirection(
        Point2d location,
        Direction direction,
        Dictionary<Point2d, CellType> map)
    {
        var status = await PerformMove(direction);
        map.Add(location, status);

        if (status is not CellType.Wall)
        {
            await Discover(location, map);
            var reversed = direction.Reverse();
            await PerformMove(reversed);
        }
    }

    private async Task<CellType> PerformMove(Direction direction)
    {
        var val = direction.DirectionType switch
        {
            DirectionType.North => 1,
            DirectionType.East => 4,
            DirectionType.South => 2,
            DirectionType.West => 3,
            _ => throw new NotImplementedException(),
        };

        await computer.Input.Writer.WriteAsync(val);
        var cellType = await computer.Output.Reader.ReadAsync();

        return (CellType)cellType;
    }
}
