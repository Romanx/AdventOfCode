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
    }
}
