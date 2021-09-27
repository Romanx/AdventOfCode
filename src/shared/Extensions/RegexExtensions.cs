using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shared
{
    public static class RegexExtensions
    {
        public static IEnumerable<Match> MatchesOverlapped(this Regex regex, string input)
        {
            var match = regex.Match(input);
            while (match.Success)
            {
                yield return match;
                match = regex.Match(input, match.Index + 1);
            }
        }
    }
}
