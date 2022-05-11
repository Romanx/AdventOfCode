using System;
using System.Collections.Specialized;

namespace Shared
{
    public static class BitVector32Extensions
    {
        public static BitVector32 FromBinaryString(ReadOnlySpan<char> input)
        {
            var vector = new BitVector32(0);

            for (var i = 0; i < input.Length; i++)
            {
                var target = 1 << (input.Length - i) - 1;
                vector[target] = input[i] is '1';
            }

            return vector;
        }

        public static string ToBinaryString(BitVector32 vector, int length = 32)
        {
            return string.Create(length, vector, static (span, vector) =>
            {
                var locdata = unchecked(vector.Data);

                // If the length isn't 32 then skip the leading data until the section we want
                locdata <<= (32 - span.Length);
                for (var i = 0; i < span.Length; i++)
                {
                    span[i] = (locdata & 0x80000000) != 0 ? '1' : '0';
                    locdata <<= 1;
                }
            });
        }
    }
}
