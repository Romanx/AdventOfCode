using System;

namespace Shared
{
    public static class StringExtensions
    {
        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            return string.Create(str.Length - length + replace.Length, (str, index, length, replace),
                (span, state) =>
                {
                    state.str.AsSpan()[..state.index].CopyTo(span);
                    state.replace.AsSpan().CopyTo(span[state.index..]);
                    state.str.AsSpan()[(state.index + state.length)..].CopyTo(span[(state.index + state.replace.Length)..]);
                });
        }

        public static string ReplaceCharAt(this string str, int index, char replace)
        {
            return string.Create(str.Length, (str, index, replace),
                (span, state) =>
                {
                    state.str.AsSpan().CopyTo(span);
                    span[index] = replace;
                });
        }

        public static char[,] As2DArray(this string str)
        {
            var lines = str.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            var array = new char[lines.Length, lines[0].Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y].AsSpan();
                for (var x = 0; x < line.Length; x++)
                {
                    array[y, x] = line[x];
                }
            }

            return array;
        }
    }
}
