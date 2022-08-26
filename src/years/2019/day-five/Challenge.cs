using System.Threading.Tasks;
using Helpers;
using MoreLinq;
using Shared2019;

namespace DayFive2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 5), "Sunny with a Chance of Asteroids");

    public async Task PartOne(IInput input, IOutput output)
    {
        var memory = input.AsIntcodeProgram();

        var computer = new IntcodeComputer(memory);
        await computer.Input.Writer.WriteAsync(1);

        var result = await computer.RunAndGetOutput();
        var diagnosticCode = result[^1];

        output.WriteProperty("Computer result", diagnosticCode);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var memory = input.AsIntcodeProgram();

        var computer = new IntcodeComputer(memory);
        await computer.Input.Writer.WriteAsync(5);

        var result = await computer.RunAndGetOutput();
        var diagnosticCode = result[0];

        output.WriteProperty("Computer result", diagnosticCode);
    }
}
