using System;
using System.Collections.Generic;
using Shared.Grid;

namespace Shared;

public static class CoordinateSystem
{
    /// <summary>
    /// This is the advent of code explicit override for this.
    /// Mostly we're parsing from screen layout and only want to convert the Y coordinates as Y positive.
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static IEnumerable<Point2d> ConvertScreenToCartesian(IEnumerable<Point2d> points)
        => ConvertScreenToCartesian(points, Offset.None, Offset.Full);

    /// <summary>
    /// This is the full bells and whistles converter where you can chose
    /// which portions to offset based on your input
    /// </summary>
    /// <param name="points"></param>
    /// <param name="xOffsetType"></param>
    /// <param name="yOffsetType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IEnumerable<Point2d> ConvertScreenToCartesian(
        IEnumerable<Point2d> points,
        Offset xOffsetType = Offset.Middle,
        Offset yOffsetType = Offset.Middle)
    {
        var area = Area2d.Create(points);
        var width = area.XRange.Max;
        var height = area.YRange.Max;

        var (xOffset, yOffset) = (xOffsetType, yOffsetType) switch
        {
            (Offset.None, Offset.None) => (0, 0),
            (Offset.None, Offset.Middle) => (0, height / 2),
            (Offset.None, Offset.Full) => (0, height),
            (Offset.Middle, Offset.None) => (width / 2, 0),
            (Offset.Middle, Offset.Middle) => (width / 2, height / 2),
            (Offset.Middle, Offset.Full) => (width / 2, height),
            (Offset.Full, Offset.None) => (width, 0),
            (Offset.Full, Offset.Middle) => (width, height / 2),
            (Offset.Full, Offset.Full) => (width, height),
            _ => throw new NotImplementedException(),
        };

        foreach (var point in points)
        {
            var x = point.X + xOffset;
            var y = yOffset - point.Y;

            yield return (x, y);
        }
    }

    public static IEnumerable<(Point2d Original, Point2d Screen)> ConvertCartesianToScreen(IEnumerable<Point2d> points)
    {
        var (xRange, yRange) = Area2d.Create(points);

        foreach (var point in points)
        {
            var x = point.X + int.Abs(xRange.Min);
            var y = yRange.Max - point.Y;

            yield return (point, (x, y));
        }
    }

    public enum Offset
    {
        None,
        Middle,
        Full
    }
}
