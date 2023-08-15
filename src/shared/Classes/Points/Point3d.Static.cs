using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PCRE;

namespace Shared;

public readonly partial record struct Point3d
{
    private static readonly PcreRegex regex = new(@"([-+]?[0-9]+),\s*([-+]?[0-9]+),\s*([-+]?[0-9]+)", PcreOptions.Compiled);

    public static implicit operator Point3d((int X, int Y, int Z) i) => new(i.X, i.Y, i.Z);

    public static implicit operator Point2d(Point3d point) => new(point.X, point.Y);

    public static Point3d Parse(string s, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point3d result)
        => TryParse(s.AsSpan(), provider, out result);

    public static Point3d Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        if (TryParse(s, provider, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"Unable to parse Point3d from '{s}'");
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Point3d result)
    {
        var match = regex.Match(s);
        if (match.Success)
        {
            var x = int.Parse(match.Groups[1].Value);
            var y = int.Parse(match.Groups[2].Value);
            var z = int.Parse(match.Groups[3].Value);

            result = new Point3d(x, y, z);
            return true;
        }

        result = Origin;
        return false;
    }

    public static AdjacentPoints3d Adjacent(Point3d centre) => new(centre);

    public static AdjacentPoints3d Adjacent(Point3d centre, IReadOnlySet<Point3d> points) => new(centre, points);
}
