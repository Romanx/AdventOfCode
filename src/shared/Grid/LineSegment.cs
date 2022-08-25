using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Shared.Helpers;

namespace Shared.Grid;

public readonly record struct LineSegment
{
    private LineSegment(Point2d start, Point2d end, LineType type)
    {
        Start = start;
        End = end;
        Type = type;
    }

    public IEnumerable<Point2d> Points => CalculatePoints();

    public Point2d Start { get; }

    public Point2d End { get; }

    public LineType Type { get; }

    public bool Intersects(LineSegment other, [NotNullWhen(true)] out Point2d? intersection)
    {
        float s10_x = End.X - Start.X;
        float s10_y = End.Y - Start.Y;
        float s32_x = other.End.X - other.Start.X;
        float s32_y = other.End.Y - other.Start.Y;

        var denom = s10_x * s32_y - s32_x * s10_y;
        if (denom == 0)
        {
            intersection = default;
            return false; // Collinear
        }

        var denomPositive = denom > 0;

        float s02_x = Start.X - other.Start.X;
        float s02_y = Start.Y - other.Start.Y;
        var s_numer = s10_x * s02_y - s10_y * s02_x;
        if ((s_numer < 0) == denomPositive)
        {
            intersection = default;
            return false; // No collision
        }

        var t_numer = s32_x * s02_y - s32_y * s02_x;
        if ((t_numer < 0) == denomPositive)
        {
            intersection = default;
            return false; // No collision
        }

        if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
        {
            intersection = default;
            return false; // No collision
        }

        // Collision detected
        var t = t_numer / denom;

        var x = Start.X + (t * s10_x);
        var y = Start.Y + (t * s10_y);

        intersection = new Point2d((int)x, (int)y);
        return true;
    }

    private IEnumerable<Point2d> CalculatePoints()
    {
        var x = (End.X - Start.X) switch
        {
            0 => 0,
            > 0 => 1,
            < 0 => -1,
        };

        var y = (End.Y - Start.Y) switch
        {
            0 => 0,
            > 0 => 1,
            < 0 => -1,
        };
        var adjustment = new Point2d(x, y);

        var start = Start;
        while (start != End)
        {
            yield return start;
            start += adjustment;
        }
        yield return End;
    }

    public static LineSegment Create(Point2d start, Point2d end)
    {
        if (start.Y == end.Y)
        {
            return new LineSegment(start, end, LineType.Horizontal);
        }
        else if (start.X == end.X)
        {
            return new LineSegment(start, end, LineType.Vertical);
        }

        var slope = PointHelpers.SlopeBetweenTwoPoints(start, end);
        if (slope is 1 or -1)
        {
            return new LineSegment(start, end, LineType.Diagonal);
        }

        throw new InvalidOperationException("There isn't a straight line between the points!");
    }
}

public enum LineType
{
    Unknown,
    Horizontal,
    Vertical,
    Diagonal,
}
