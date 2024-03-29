﻿using System.Text.RegularExpressions;
using static Shared.AlphabetHelper;

namespace DayEleven2015
{
    class PasswordGenerator
    {
        private static readonly char[] invalidCharacters = new[] { 'i', 'o', 'l' };
        private static readonly Regex pairsRegex = new(@"([a-z])\1.*([a-z])\2");
        private static readonly char[][] straights = GenerateStraights().ToArray();

        public static string GeneratePassword(string password)
        {
            var next = password;
            while (true)
            {
                next = NextPossiblePassword(next);
                if (PasswordGenerator.ValidatePassword(next))
                {
                    return next;
                }
            }

            static string NextPossiblePassword(ReadOnlySpan<char> password)
            {
                if (password.Length == 0)
                {
                    return string.Empty;
                }

                var remaining = password[0..^1].ToString();
                var next = Lowercase.IndexOf(password[^1]) + 1;

                return next < Lowercase.Length
                    ? remaining + Lowercase[next]
                    : NextPossiblePassword(remaining) + Lowercase[0];
            }
        }

        private static bool ValidatePassword(string password)
        {
            var hasStraight = HasStraight(password);
            var hasRestrictedCharacter = HasInvalid(password);
            var hasTwoNonOverlappingPairs = HasOverlappingPairs(password);

            return !hasRestrictedCharacter &&
                    hasStraight &&
                    hasTwoNonOverlappingPairs;
        }

        private static IEnumerable<char[]> GenerateStraights()
        {
            for (var i = 0; i < Lowercase.Length; i++)
            {
                if (i + 3 <= Lowercase.Length)
                {
                    yield return new[]
                    {
                        Lowercase[i],
                        Lowercase[i + 1],
                        Lowercase[i + 2],
                    };
                }
            }
        }

        private static bool HasStraight(ReadOnlySpan<char> password)
        {
            foreach (var straight in straights)
            {
                if (password.Contains(straight, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasOverlappingPairs(string password)
        {
            return pairsRegex.IsMatch(password);
        }

        private static bool HasInvalid(ReadOnlySpan<char> password) => password.IndexOfAny(invalidCharacters) != -1;
    }
}
