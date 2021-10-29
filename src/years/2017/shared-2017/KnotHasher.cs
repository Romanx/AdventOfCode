using System;
using System.Linq;

namespace Shared
{
    public static class KnotHasher
    {
        public static byte[] Hash(byte[] lengths)
        {
            var withPostfix = lengths
                .Concat(new byte[] { 17, 31, 73, 47, 23 })
                .ToArray();

            Span<byte> array = GenerateArray();
            var pointer = 0;
            var skipSize = 0;

            for (var i = 0; i < 64; i++)
            {
                ApplyRound(array, withPostfix, ref pointer, ref skipSize);
            }

            var dense = ToDenseHash(array);

            return dense;

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

        public static void ApplyRound(
            Span<byte> array,
            ReadOnlySpan<byte> lengths,
            ref int pointer,
            ref int skipSize)
        {
            foreach (var length in lengths)
            {
                if (pointer + length > array.Length)
                {
                    HandleOverflowingTwist(array, pointer, length);
                }
                else
                {
                    var slice = array[pointer..(pointer + length)];
                    slice.Reverse();
                }

                pointer = (pointer + length + skipSize) % array.Length;
                skipSize++;
            }

            static void HandleOverflowingTwist(Span<byte> array, int pointer, int length)
            {
                var end = array[pointer..];
                var remaining = length - end.Length;
                var start = array[..remaining];

                Span<byte> scratch = stackalloc byte[length];
                end.CopyTo(scratch);
                start.CopyTo(scratch[end.Length..]);
                scratch.Reverse();

                scratch[0..end.Length].CopyTo(end);
                scratch[end.Length..].CopyTo(start);
            }
        }

        private static byte[] GenerateArray()
        {
            var bytes = new byte[256];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)i;
            }

            return bytes;
        }
    }
}
