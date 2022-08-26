using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Computer;

namespace Shared2019;

public static class ParseExtensions
{
    public static ImmutableArray<long> AsIntcodeProgram(this IInput input)
        => input.Content.Transform(static str => IntcodeParser.Parse(str));
}
