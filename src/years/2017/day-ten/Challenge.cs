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

            ApplyTwists(@string, lengths, ref pointer, ref skipSize);

            output.WriteProperty("list", string.Join(", ", @string.ToArray()));
            output.WriteProperty("First", @string[0]);
            output.WriteProperty("Second", @string[1]);
            output.WriteProperty("Result", @string[0] * @string[1]);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const int StringLength = 256;
            Span<byte> @string = MoreLinq.MoreEnumerable.Generate<byte>(0, i => (byte)(i + 1))
                .Take(StringLength)
                .ToArray();

            var ascii = input.Content.ParseAscii()
                .Concat(new byte[] { 17, 31, 73, 47, 23 })
                .ToArray();

            var pointer = 0;
            var skipSize = 0;

            for (var i = 0; i < 64; i++)
            {
                ApplyTwists(@string, ascii, ref pointer, ref skipSize);
            }

            var dense = ToDenseHash(@string);

            var hex = Convert.ToHexString(dense).ToLowerInvariant();

            output.WriteProperty("Hex", hex);

            static byte[] ToDenseHash(Span<byte> input)
            {
                ReadOnlySpan<byte> slice = input;
                var result = new byte[16];
                for (var i = 0; i < 16; i++)
                {
                    result[i] = XOR(slice[0..16]);
                    slice = slice[16..];
                }

                return result;
            }

            static byte XOR(ReadOnlySpan<byte> input)
            {
                var value = input[0];
                for (var i = 1; i < input.Length; i++)
                {
                    value ^= input[i];
                }

                return value;
            }
        }

        static void ApplyTwists(
            Span<byte> @string,
            Span<byte> lengths,
            ref int pointer,
            ref int skipSize)
        {
            foreach (var length in lengths)
            {
                if (pointer + length > @string.Length)
                {
                    HandleOverflowingTwist(@string, pointer, length);
                }
                else
                {
                    var slice = @string[pointer..(pointer + length)];
                    slice.Reverse();
                }

                pointer = (pointer + length + skipSize) % @string.Length;
                skipSize++;
            }

            static void HandleOverflowingTwist(Span<byte> @string, int pointer, int length)
            {
                var end = @string[pointer..];
                var remaining = length - end.Length;
                var start = @string[..remaining];

                Span<byte> scratch = stackalloc byte[length];
                end.CopyTo(scratch);
                start.CopyTo(scratch[end.Length..]);
                scratch.Reverse();

                scratch[0..end.Length].CopyTo(end);
                scratch[end.Length..].CopyTo(start);
            }
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
