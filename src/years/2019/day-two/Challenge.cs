using System.Threading.Tasks;
using Helpers;
using Shared2019;

namespace DayTwo2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 2), "1202 Program Alarm");

    public async Task PartOne(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();

        var builder = program.ToBuilder();
        builder[1] = 12;
        builder[2] = 2;

        var computer = new IntcodeComputer(builder.ToImmutable());
        var result = await computer.RunAndGetOutput();

        output.WriteProperty("Computer Result", result[0]);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var scratch = new long[program.Length];

        foreach (var (noun, verb) in GenerateCandidates())
        {
            program.CopyTo(scratch);

            scratch[1] = noun;
            scratch[2] = verb;

            var computer = new IntcodeComputer(scratch.ToImmutableArray());
            var result = (await computer.RunAndGetOutput())[0];

            if (result == 19690720)
            {
                output.WriteProperty("Noun", noun);
                output.WriteProperty("Verb", verb);
                output.WriteProperty("Result", $"100 * {noun} + {verb} = {100 * noun + verb}");
                break;
            }
        }

        static IEnumerable<(int Noun, int Verb)> GenerateCandidates()
        {
            foreach (var noun in Enumerable.Range(0, 100))
            {
                foreach (var verb in Enumerable.Range(0, 100))
                {
                    yield return (noun, verb);
                }
            }
        }
    }
}

