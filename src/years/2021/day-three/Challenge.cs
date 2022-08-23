using System.Collections.Specialized;

namespace DayThree2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 3), "Binary Diagnostic");

    public void PartOne(IInput input, IOutput output)
    {
        var numbers = input.Lines.AsArray();

        var bitInformation = ParseBitIndexCounts(numbers);

        var gammaRateBits = CalculateGammaRate(bitInformation);
        var gammaRateValue = BitVector32Extensions.FromBinaryString(gammaRateBits).Data;

        var epsilonRateBits = CalculateEpsilonRate(bitInformation);
        var epsilonRateValue = BitVector32Extensions.FromBinaryString(epsilonRateBits).Data;

        output.WriteProperty("Gamma Rate Bits", gammaRateBits);
        output.WriteProperty("Epsilon Rate Bits", epsilonRateBits);
        output.WriteProperty("Gamma Rate Value", gammaRateValue);
        output.WriteProperty("Epsilon Rate Value", epsilonRateValue);

        output.WriteProperty("Power Consumption", gammaRateValue * epsilonRateValue);

        static string CalculateGammaRate(Dictionary<int, PositionInfo> bitIndexCounts)
        {
            return string.Create(bitIndexCounts.Count, bitIndexCounts, (span, state) =>
            {
                foreach (var (index, info) in bitIndexCounts)
                {
                    span[index] = info.NumberOfOnes > info.NumberOfZeros
                        ? '1'
                        : '0';
                }
            });
        }

        static string CalculateEpsilonRate(Dictionary<int, PositionInfo> bitIndexCounts)
        {
            return string.Create(bitIndexCounts.Count, bitIndexCounts, (span, state) =>
            {
                foreach (var (index, info) in bitIndexCounts)
                {
                    span[index] = info.NumberOfOnes > info.NumberOfZeros
                        ? '0'
                        : '1';
                }
            });
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var numbers = input.Lines.AsArray();

        var oxygenRating = FindTargetValue(numbers, OxygenCriteria);
        var scrubberRating = FindTargetValue(numbers, ScrubberCriteria);

        output.WriteProperty("Oxygen Rating", oxygenRating);
        output.WriteProperty("CO2 scrubber rating", scrubberRating);
        output.WriteProperty("Life Support Rating", oxygenRating * scrubberRating);

        static int FindTargetValue(
            string[] numbers,
            Func<int, string, PositionInfo, bool> criteriaFunction)
        {
            var index = 0;
            var current = new List<string>(numbers);

            while (current.Count > 1)
            {
                var scratch = new List<string>(current);
                var info = CalculatePositionInfo(index, scratch);

                foreach (var number in current)
                {
                    if (criteriaFunction(index, number, info) is false)
                    {
                        scratch.Remove(number);
                    }
                }

                current = scratch;
                index++;
            }

            return Convert.ToInt32(current[0], 2);
        }

        static bool OxygenCriteria(int index, string number, PositionInfo info)
        {
            if (info.NumberOfOnes >= info.NumberOfZeros)
            {
                return number[index] == '1';
            }
            else
            {
                return number[index] == '0';
            }
        }

        static bool ScrubberCriteria(int index, string number, PositionInfo info)
        {
            if (info.NumberOfZeros <= info.NumberOfOnes)
            {
                return number[index] == '0';
            }
            else
            {
                return number[index] == '1';
            }
        }
    }

    static Dictionary<int, PositionInfo> ParseBitIndexCounts(IEnumerable<string> lines)
    {
        var dictionary = new Dictionary<int, PositionInfo>();
        var arr = lines.ToArray();
        var numberLength = arr[0].Length;

        for (var i = 0; i < numberLength; i++)
        {
            var info = CalculatePositionInfo(i, arr);

            dictionary[i] = info;
        }

        return dictionary;
    }

    static PositionInfo CalculatePositionInfo(int index, IEnumerable<string> numbers)
    {
        var zeroes = 0;
        var ones = 0;
        foreach (var number in numbers)
        {
            var span = number.AsSpan();
            var value = span[index];
            if (value is '1')
            {
                ones++;
            }
            else
            {
                zeroes++;
            }
        }

        return new PositionInfo(ones, zeroes);
    }
}

readonly record struct PositionInfo(int NumberOfOnes, int NumberOfZeros);
