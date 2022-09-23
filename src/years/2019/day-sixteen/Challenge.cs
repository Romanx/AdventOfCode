using System.Linq;
using MoreLinq;
using Shared.Extensions;

namespace DaySixteen2019;

public class Challenge : Shared.Challenge
{
    private static readonly ImmutableArray<int> basePattern = ImmutableArray.Create(0, 1, 0, -1);

    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 16), "Flawed Frequency Transmission");

    public void PartOne(IInput input, IOutput output)
    {
        var phases = 100;
        var signal = input.Content
            .CharactersToInt()
            .ToImmutableArray();

        var result = RunLine(phases, signal);

        output.WriteProperty("Final Signal", string.Join("", result.Take(8)));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var signal = input.Content
            .CharactersToInt()
            .Repeat(10000)
            .ToArray();

        var offset = int.Parse(input.Content.AsSpan()[..7]);
        var newSequence = new int[signal.Length];

        for (var phase = 0; phase < 100; phase++)
        {
            var sum = 0;
            for (var i = signal.Length - 1; i >= signal.Length / 2; i--)
            {
                sum += signal[i];
                newSequence[i] = sum.DigitAt(1);
            }

            signal = newSequence;
        }

        var shortened = signal.Skip(offset).Take(8);

        output.WriteProperty("Offset", offset);
        output.WriteProperty("Final Signal", string.Join("", shortened));
    }

    private ImmutableArray<int> RunLine(int phaseCount, ImmutableArray<int> startingSignal)
    {
        var signal = startingSignal;
        for (var i = 0; i < phaseCount; i++)
        {
            signal = RunPhase(signal);
        }

        return signal;
    }

    private static ImmutableArray<int> RunPhase(ImmutableArray<int> signal)
    {
        var result = ImmutableArray.CreateBuilder<int>(signal.Length);

        for (var i = 0; i < signal.Length; i++)
        {
            result.Add(ApplyPattern(i, signal));
        }

        return result.MoveToImmutable();

        static int ApplyPattern(int index, ImmutableArray<int> signal)
        {
            var pattern = GeneratePattern(index + 1, signal.Length);

            var total = 0;
            for (var i = 0; i < signal.Length; i++)
            {
                var value = signal[i];
                var patternItem = pattern[i];

                total += value * patternItem;
            }

            return total.DigitAt(1);
        }
    }

    public static int[] GeneratePattern(int patternCount, int signalCount)
    {
        var res = Enumerable.Empty<int>();

        foreach (var item in basePattern)
        {
            res = res.Concat(Enumerable.Repeat(item, patternCount));
        }

        return res.Repeat().Skip(1).Take(signalCount).ToArray();
    }
}
