using System.Text;
using System.Threading.Tasks;
using Helpers;
using Shared.Grid;
using Shared2019;
using Spectre.Console;

namespace DayNineteen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 19), "Tractor Beam");

    public async Task PartOne(IInput input, IOutput output)
    {
        var droid = new Droid(input.AsIntcodeProgram());
        var area = Area2d.Create((0, 0), (49, 49));

        var set = new HashSet<Point2d>();

        foreach (var point in area.Items)
        {
            if (await droid.Test(point))
            {
                set.Add(point);
            }
        }

        output.WriteProperty("Number of points", set.Count);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var droid = new Droid(input.AsIntcodeProgram());

        // Since we need to find where the top right and bottom
        // left are both in the beam
        // ****B
        // *****
        // *****
        // *****
        // A****

        var x = 0;
        var y = 0;

        while (true)
        {
            // Find the bottom left
            while (!await droid.Test((x, y + 99)))
            {
                x++;
            }

            // If the top right is in the beam we're done!
            if (await droid.Test((x + 99, y)))
            {
                break;
            }

            y++;
        }

        output.WriteProperty($"Result of calculation is", (x * 10_000) + y);
    }
}

public class Droid
{
    private readonly ImmutableArray<long> program;

    public Droid(ImmutableArray<long> program)
    {
        this.program = program;
    }

    public async Task<bool> Test(Point2d point)
    {
        if (point.X < 0 || point.Y < 0)
        {
            return false;
        }

        var computer = new IntcodeComputer(program);
        computer.Input.Writer.TryWrite(point.X);
        computer.Input.Writer.TryWrite(point.Y);

        var output = await computer.RunAndGetOutput();

        return output[0] is 1;
    }
}
