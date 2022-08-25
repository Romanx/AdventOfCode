using Helpers;
using Shared2019;

namespace DayFive2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 5), "Sunny with a Chance of Asteroids");

    public void PartOne(IInput input, IOutput output)
    {
        var memory = input.AsIntcodeProgram();

        var computer = new IntcodeComputer(memory);
        computer.Input.Enqueue(1);
        computer.Run();

        var result = computer.Output.Last();

        output.WriteProperty("Computer result", result);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var memory = input.AsIntcodeProgram();

        var computer = new IntcodeComputer(memory);
        computer.Input.Enqueue(5);
        computer.Run();

        var result = computer.Output.Last();

        output.WriteProperty("Computer result", result);
    }
}
