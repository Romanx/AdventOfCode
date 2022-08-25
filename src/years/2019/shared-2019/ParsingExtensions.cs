using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared2019;

public static class ParseExtensions
{
    public static ImmutableArray<long> AsIntcodeProgram(this IInput input)
        => input.Content.Transform(static str =>
        {
            return str.Split(',').Select(long.Parse).ToImmutableArray();
        });
}
