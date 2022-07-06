using System;
using System.Collections.Generic;

namespace Shared
{
    public static class AlphabetHelper
    {
        private const string LowercaseString = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AlphabetString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static ReadOnlySpan<char> Lowercase => LowercaseString.AsSpan();

        public static IEnumerable<char> LowercaseEnumerable => LowercaseString;

        public static ReadOnlySpan<char> Uppercase => UppercaseString.AsSpan();

        public static IEnumerable<char> UppercaseEnumerable => UppercaseString;

        public static ReadOnlySpan<char> Alphabet => AlphabetString.AsSpan();

        public static IEnumerable<char> AlphabetEnumerable => AlphabetString;
    }
}
