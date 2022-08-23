namespace DayTwentyFour2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 24), "Arithmetic Logic Unit");

    public void PartOne(IInput input, IOutput output)
    {
        var parameters = input.Lines.ParseMagicParameters();
        var (_, max) = Solve(parameters);

        output.WriteProperty("Maximum value", max);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var parameters = input.Lines.ParseMagicParameters();
        var (min, _) = Solve(parameters);

        output.WriteProperty("Minimum value", min);
    }

    private static (long Min, long Max) Solve(ImmutableArray<Parameters> parameters)
    {
        var zValues = new Dictionary<long, (long Min, long Max)>
        {
            [0] = (0, 0)
        };

        foreach (var parameter in parameters)
        {
            var zValuesThisRound = new Dictionary<long, (long Min, long Max)>();

            foreach (var (z, minMax) in zValues)
            {
                for (var digit = 1L; digit <= 9; digit++)
                {
                    var newValueForZ = MagicFunction(parameter, z, digit);
                    if (parameter.A == 1 || (parameter.A == 26 && newValueForZ < z))
                    {
                        var (existingMin, existingMax) = zValuesThisRound.GetValueOrDefault(newValueForZ, (long.MaxValue, long.MinValue));

                        zValuesThisRound[newValueForZ] = (
                            Math.Min(existingMin, minMax.Min * 10 + digit),
                            Math.Max(existingMax, minMax.Max * 10 + digit));
                    }
                }
            }
            zValues = zValuesThisRound;
        }

        return zValues[0];
    }

    private static long MagicFunction(Parameters parameters, long z, long w)
    {
        return (z % 26 + parameters.B != w)
            ? ((z / parameters.A) * 26) + w + parameters.C
            : z / parameters.A;
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Parameters> ParseMagicParameters(this IInputLines lines)
    {
        var arr = lines.AsArray();

        var builder = ImmutableArray.CreateBuilder<Parameters>();

        foreach (var chunk in arr.Chunk(18))
        {
            var a = int.Parse(chunk[4].Split(' ')[^1]);
            var b = int.Parse(chunk[5].Split(' ')[^1]);
            var c = int.Parse(chunk[15].Split(' ')[^1]);

            builder.Add(new Parameters(a, b, c));
        }

        return builder.ToImmutable();
    }
}

readonly record struct Parameters(long A, long B, long C);
