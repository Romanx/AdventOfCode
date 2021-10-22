using System.Linq;
using NodaTime;
using Shared;

namespace DayTwentyFive2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 25), "Combo Breaker");

        public override void PartOne(IInput input, IOutput output)
        {
            var (cardPublicKey, devicePublicKey) = input.Parse();

            var cardLoopSize = FindLoopSize(cardPublicKey);
            var deviceLoopSize = FindLoopSize(devicePublicKey);

            output.WriteProperty("Card Loop Size", cardLoopSize);
            output.WriteProperty("Device Loop Size", deviceLoopSize);
            output.WriteProperty("Device Encryption Key", TransformSubjectNumber(cardLoopSize, devicePublicKey));
            output.WriteProperty("Card Encryption Key", TransformSubjectNumber(deviceLoopSize, cardPublicKey));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            output.WriteProperty("There is none! We finished it!", string.Empty);
        }

        static long FindLoopSize(long publicKey)
        {
            var value = 1L;
            var loopSize = 0;
            for (; value != publicKey; loopSize++)
            {
                value *= 7L;
                value %= 20201227;
            }

            return loopSize;
        }

        static long TransformSubjectNumber(long loopSize, long subjectNumber)
        {
            var val = 1L;
            for (var l = 1; l <= loopSize; l++)
            {
                val *= subjectNumber;
                val %= 20201227;
            }

            return val;
        }
    }

    internal static class ParseExtensions
    {
        public static (long CardPublicKey, long DevicePublicKey) Parse(this IInput input)
        {
            var lines = input.Lines.AsMemory().ToArray();

            return (long.Parse(lines[0].Span), long.Parse(lines[1].Span));
        }
    }
}
