using System.Threading.Tasks;
using Helpers;
using Shared2019;

namespace DayNine2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 9), "Sensor Boost");

    public async Task PartOne(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var computer = new IntcodeComputer(program);
        computer.Input.Writer.TryWrite(1);

        var computerOutput = await computer.RunAndGetOutput();
        var result = computerOutput[0];

        output.WriteProperty("Computer result", result);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var computer = new IntcodeComputer(program);
        computer.Input.Writer.TryWrite(2);

        var computerOutput = await computer.RunAndGetOutput();
        var result = computerOutput[0];

        output.WriteProperty("Computer result", result);
    }
}
