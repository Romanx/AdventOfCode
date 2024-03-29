﻿using System;
using CommunityToolkit.HighPerformance;

namespace Shared;

public static  class Span2dExtensions
{
    public static bool Contains<T>(this T[,] array, Point2d point)
        => Contains(array.AsSpan2D(), point);

    public static bool Contains<T>(this Memory2D<T> memory, Point2d point)
        => Contains(memory.Span, point);

    public static bool Contains<T>(this ReadOnlyMemory2D<T> memory, Point2d point)
        => Contains(memory.Span, point);

    public static bool Contains<T>(this ReadOnlySpan2D<T> span, Point2d point)
    {
        if (point.Y < 0 || point.Y >= span.Height)
        {
            return false;
        }
        else if (point.X < 0 || point.X >= span.Width)
        {
            return false;
        }

        return true;
    }

    public static bool Contains<T>(this Span2D<T> span, Point2d point)
    {
        if (point.Y < 0 || point.Y >= span.Height)
        {
            return false;
        }
        else if (point.X < 0 || point.X >= span.Width)
        {
            return false;
        }

        return true;
    }

    public static T At<T>(this T[,] array, Point2d point)
        => At(array.AsSpan2D(), point);

    public static T At<T>(this Memory2D<T> memory, Point2d point)
        => At(memory.Span, point);

    public static T At<T>(this ReadOnlyMemory2D<T> memory, Point2d point)
        => At(memory.Span, point);

    public static T At<T>(this ReadOnlySpan2D<T> array, Point2d point)
        => array[point.Y, point.X];

    public static T At<T>(this Span2D<T> array, Point2d point)
        => array[point.Y, point.X];

    public static bool TryGetAt<T>(this T[,] array, Point2d point, out T? result)
    {
        if (Contains(array, point))
        {
            result = At(array, point);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryGetAt<T>(this Memory2D<T> memory, Point2d point, out T? result)
    {
        if (Contains(memory, point))
        {
            result = At(memory, point);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryGetAt<T>(this ReadOnlyMemory2D<T> memory, Point2d point, out T? result)
    {
        if (Contains(memory, point))
        {
            result = At(memory, point);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryGetAt<T>(this ReadOnlySpan2D<T> span, Point2d point, out T? result)
    {
        if (Contains(span, point))
        {
            result = At(span, point);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryGetAt<T>(this Span2D<T> span, Point2d point, out T? result)
    {
        if (Contains(span, point))
        {
            result = At(span, point);
            return true;
        }

        result = default;
        return false;
    }
}
