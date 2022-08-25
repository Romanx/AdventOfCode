using Helpers;

namespace DayTwo2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 2), "1202 Program Alarm");

    public void PartOne(IInput input, IOutput output)
    {
        var program = input.AsProgram();

        var builder = program.ToBuilder();
        builder[1] = 12;
        builder[2] = 2;

        var computer = new IntcodeComputer(builder.ToImmutable());
        computer.Run();
        var result = computer.Output.Dequeue();

        output.WriteProperty("Computer Result", result);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var program = input.AsProgram();
        var scratch = new long[program.Length];

        foreach (var (noun, verb) in GenerateCandidates())
        {
            program.CopyTo(scratch);

            scratch[1] = noun;
            scratch[2] = verb;

            var computer = new IntcodeComputer(scratch.ToImmutableArray());
            computer.Run();
            var result = computer.Output.Dequeue();

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

internal static class ParseExtensions
{
    public static ImmutableArray<long> AsProgram(this IInput input)
        => input.Content.Transform(static str =>
        {
            return str.Split(',').Select(long.Parse).ToImmutableArray();
        });
}
