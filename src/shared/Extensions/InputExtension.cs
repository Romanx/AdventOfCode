using System.Collections.Generic;
using System.Linq;

namespace Shared
{
    public static class InputExtension
    {
        public static IEnumerable<int> FromIntString(this IInput input) => input
            .AsString()
            .ToCharArray()
            .Select(c => int.Parse(c.ToString()));
    }
}
