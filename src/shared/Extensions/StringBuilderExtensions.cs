using System;
using System.Text;

namespace Shared.Extensions
{
    public static class StringBuilderExtensions
    {
        public static bool EndsWith(this StringBuilder sb, string test, StringComparison comparison)
        {
            if (sb.Length < test.Length)
                return false;

            var end = sb.ToString(sb.Length - test.Length, test.Length);
            return end.Equals(test, comparison);
        }
    }
}
