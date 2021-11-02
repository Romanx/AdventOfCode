using System;
using System.Linq;
using NodaTime;
using Shared;

namespace DaySixteen2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 16), "Dragon Checksum");

        public override void PartOne(IInput input, IOutput output)
        {
            var seed = input.Parse();

            var data = CreateData(seed.Span, 272);
            var checksum = GenerateChecksum(data);

            output.WriteProperty("Checksum", Print(checksum.ToArray()));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var seed = input.Parse();

            var data = CreateData(seed.Span, 35_651_584);
            var checksum = GenerateChecksum(data);

            output.WriteProperty("Checksum", Print(checksum.ToArray()));
        }

        private static string Print(ReadOnlyMemory<bool> memory)
        {
            return string.Create(memory.Length, memory, static (span, state) =>
            {
                var stateSpan = state.Span;
                for (var i = 0; i < state.Length; i++)
                {
                    span[i] = stateSpan[i] ? '1' : '0';
                }
            });
        }

        static ReadOnlySpan<bool> CreateData(ReadOnlySpan<bool> seed, int length)
        {
            var data = seed;

            while (true)
            {
                data = ApplyDragonCurve(data);
                if (data.Length >= length)
                {
                    return data.Slice(0, length);
                }
            }

            static ReadOnlySpan<bool> ApplyDragonCurve(ReadOnlySpan<bool> incoming)
            {
                Span<bool> b = incoming.Length <= 1024
                    ? stackalloc bool[incoming.Length]
                    : new bool[incoming.Length];

                incoming.CopyTo(b);
                b.Reverse();
                b.Invert();

                Span<bool> result = new bool[incoming.Length * 2 + 1];
                incoming.CopyTo(result);
                result[incoming.Length + 1] = false;
                b.CopyTo(result[(incoming.Length + 1)..]);

                return result;
            }
        }

        static ReadOnlySpan<bool> GenerateChecksum(ReadOnlySpan<bool> data)
        {
            var checksum = Checksum(data);

            while (checksum.Length % 2 == 0)
            {
                checksum = Checksum(checksum);
            }

            return checksum;

            static ReadOnlySpan<bool> Checksum(ReadOnlySpan<bool> x)
            {
                Span<bool> checksum = new bool[x.Length / 2];

                var index = 0;
                for (var i = 0; i < x.Length; i += 2)
                {
                    checksum[index] = x[i] == x[i + 1];
                    index++;
                }

                return checksum;
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ReadOnlyMemory<bool> Parse(this IInput input)
        {
            var str = input.Content.AsSpan();

            var array = new bool[str.Length];
            for (var i = 0; i < str.Length; i++)
            {
                array[i] = str[i] == '1';
            }

            return array;
        }
    }

    internal static class SpanExtensions
    {
        public static void Invert(this Span<bool> span)
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] ^= true;
            }
        }
    }
}
