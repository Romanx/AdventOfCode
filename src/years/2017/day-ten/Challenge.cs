using System;
using System.Linq;
using System.Text;
using NodaTime;
using Shared;

namespace DayTen2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 10), "Knot Hash");

        public override void PartOne(IInput input, IOutput output)
        {
            const int StringLength = 256;
            Span<byte> @string = MoreLinq.MoreEnumerable.Generate<byte>(0, i => (byte)(i + 1))
                .Take(StringLength)
                .ToArray();

            var lengths = input.Content.ParseLengths();
            var pointer = 0;
            var skipSize = 0;

            KnotHasher.ApplyRound(@string, lengths, ref pointer, ref skipSize);

            output.WriteProperty("list", string.Join(", ", @string.ToArray()));
            output.WriteProperty("First", @string[0]);
            output.WriteProperty("Second", @string[1]);
            output.WriteProperty("Result", @string[0] * @string[1]);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var ascii = input.Content.ParseAscii()
                .Concat(new byte[] { 17, 31, 73, 47, 23 })
                .ToArray();

            var hash = KnotHasher.Hash(ascii);

            var hex = Convert.ToHexString(hash).ToLowerInvariant();

            output.WriteProperty("Hex", hex);
        }
    }

    internal static class ParseExtensions
    {
        public static byte[] ParseLengths(this IInputContent content)
            => content.Transform(static str => str
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(byte.Parse)
                .ToArray());

        public static byte[] ParseAscii(this IInputContent content)
            => content.Transform(static str => Encoding.ASCII.GetBytes(str.Trim()));
    }
}
