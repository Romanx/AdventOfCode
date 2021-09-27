using System;

namespace Shared
{
    public static class AlphabetHelper
    {
        private const string LowercaseString = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlphabetString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static ReadOnlySpan<char> Lowercase => LowercaseString.AsSpan();

        public static ReadOnlySpan<char> Uppercase => UppercaseString.AsSpan();

        public static ReadOnlySpan<char> Alphabet => AlphabetString.AsSpan();
    }
}
