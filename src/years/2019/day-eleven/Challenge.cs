using System.Threading.Tasks;
using Helpers;
using Shared2019;
using SixLabors.ImageSharp;

namespace DayEleven2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 11), "Space Police");

    public async Task PartOne(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var robot = new HullPaintingRobot(new Dictionary<Point2d, int>());
        await robot.StartPainting(program);

        output.WriteProperty("Number of points painted", robot.Painting.Count);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var robot = new HullPaintingRobot(new Dictionary<Point2d, int>
        {
            [Point2d.Origin] = 1
        });
        await robot.StartPainting(program);

        var writer = new ImageWriter(robot.Painting);

        output.WriteImage(writer.Generate());
    }

    private sealed class ImageWriter : GridImageWriter<int>
    {
        public ImageWriter(IReadOnlyDictionary<Point2d, int> map) : base(map)
        {
        }

        protected override Color GetColorForPoint(Point2d point)
        {
            if (_map.TryGetValue(point, out var cellType))
            {
                return cellType switch
                {
                    0 => Color.Black,
                    1 => Color.White,
                    _ => throw new NotImplementedException(),
                };
            }

            return Color.Black;
        }
    }

}

public class HullPaintingRobot
{
    private readonly Dictionary<Point2d, int> state;
    private Point2d position;
    private Direction facing;

    public HullPaintingRobot(Dictionary<Point2d, int> state)
    {
        this.state = state;
        position = Point2d.Origin;
        facing = Direction.North;
    }

    public ImmutableDictionary<Point2d, int> Painting => state.ToImmutableDictionary();

    public async Task StartPainting(ImmutableArray<long> program)
    {
        var computer = new IntcodeComputer(program);

        await Task.WhenAll(
            Task.Run(RunStateStep),
            computer.Run());

        async Task RunStateStep()
        {
            while (await computer.Input.Writer.WaitToWriteAsync())
            {
                var colour = state.TryGetValue(position, out var currentColour)
                    ? currentColour
                    : 0;

                computer.Input.Writer.TryWrite(colour);

                var result = await computer.Output.Reader.WaitToReadAsync();
                if (result is false)
                {
                    return;
                }

                var outColour = await computer.Output.Reader.ReadAsync();
                state[position] = (int)outColour;

                var move = await computer.Output.Reader.ReadAsync();

                if (move == 0)
                {
                    facing = facing.Left();
                }
                else if (move == 1)
                {
                    facing = facing.Right();
                }

                position += facing;
            }
        }
    }
}
