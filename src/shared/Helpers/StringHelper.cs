using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class StringHelper
    {
        public static string Reverse(this string str)
        {
            return string.Create(str.Length, str, static (span, str) =>
            {
                str.AsSpan().CopyTo(span);
                span.Reverse();
            });
        }
    }
}
