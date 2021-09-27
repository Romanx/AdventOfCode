using System;
using NodaTime;
using PCRE;
using Shared;

namespace DayNine2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 9), "Explosives in Cyberspace");

        public override void PartOne(IInput input, IOutput output)
        {
            var decompressed = Decompressor.DecompressedLength(input.AsReadOnlyMemory().Span);

            output.WriteProperty("Decompressed Length", decompressed);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var decompressed = Decompressor.DecompressedLengthV2(input.AsReadOnlyMemory().Span);

            output.WriteProperty("Decompressed Length", decompressed);
        }
    }

    class Decompressor
    {
        private static readonly PcreRegex regex = new(@"\(([0-9]+)x([0-9]+)\)");

        public static long DecompressedLengthV2(ReadOnlySpan<char> span)
        {
            var fileLength = 0L;

            var match = regex.Match(span);
            while (match.Success)
            {
                if (match.Index != 0)
                {
                    fileLength += span[..match.Index].Trim().Length;
                }

                var spanLength = int.Parse(match[1].Value);
                var count = int.Parse(match[2].Value);

                var end = match.EndIndex;
                var slice = span[end..(end + spanLength)];
                var expandedLength = DecompressedLengthV2(slice);

                fileLength += expandedLength * count;

                span = span[(end + spanLength)..];
                match = regex.Match(span);
            }

            fileLength += span.Trim().Length;
            return fileLength;
        }

        public static long DecompressedLength(ReadOnlySpan<char> span)
        {
            var fileLength = 0L;

            var match = regex.Match(span);
            while (match.Success)
            {
                if (match.Index != 0)
                {
                    fileLength += span[..match.Index].Trim().Length;
                }

                var spanLength = int.Parse(match[1].Value);
                var count = int.Parse(match[2].Value);

                var end = match.EndIndex;
                var slice = span[end..(end + spanLength)];
                var expandedLength = slice.Length;

                fileLength += expandedLength * count;

                span = span[(end + spanLength)..];
                match = regex.Match(span);
            }

            fileLength += span.Trim().Length;
            return fileLength;
        }
    }
}
