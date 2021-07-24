﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Shared;

namespace DaySeventeen2018
{
    public class Scan
    {
        public static Point2d WaterSpring { get; } = new Point2d(500, 0);
        private readonly Dictionary<Point2d, CellType> _map;
        private readonly GridDimensions gridDimensions;

        public Scan(ImmutableHashSet<Point2d> clay)
        {
            _map = new Dictionary<Point2d, CellType>(clay.Select(p => KeyValuePair.Create(p, CellType.Clay)));
            _map.Add(WaterSpring, CellType.Spring);
            var (xRange, yRange) = Point2d.FindSpaceOfPoints(_map.Keys);
            gridDimensions = new GridDimensions(xRange.Pad(3), yRange);
        }

        public ImmutableDictionary<Point2d, CellType> Map => _map.ToImmutableDictionary();

        public void Flow(Point2d source)
        {
            var down = source + GridDirection.Down;
            var right = source + GridDirection.Right;
            var left = source + GridDirection.Left;

            if (InGrid(down) is false)
            {
                return;
            }

            if (GetCellType(down) is CellType.Sand)
            {
                _map[down] = CellType.FlowingWater;
                Flow(down);
            }

            if (GetCellType(down) is CellType.StillWater or CellType.Clay
                && InGrid(right) && GetCellType(right) == CellType.Sand)
            {
                _map[right] = CellType.FlowingWater;
                Flow(right);
            }

            if (GetCellType(down) is CellType.StillWater or CellType.Clay
                && InGrid(left) && GetCellType(left) == CellType.Sand)
            {
                _map[left] = CellType.FlowingWater;
                Flow(left);
            }

            if (HasWalls(source))
            {
                FillLeftAndRight(source);
            }
        }

        private bool HasWalls(Point2d source)
        {
            return HasWalls(source, p => p + GridDirection.Right) &&
                HasWalls(source, p => p + GridDirection.Left);

            bool HasWalls(Point2d source, Func<Point2d, Point2d> nextPoint)
            {
                var point = source;

                while (InGrid(point))
                {
                    switch (GetCellType(point))
                    {
                        case CellType.Sand:
                            return false;
                        case CellType.Clay:
                            return true;
                        default:
                            point = nextPoint(point);
                            break;
                    }
                }

                return false;
            }
        }

        private void FillLeftAndRight(Point2d source)
        {
            FillUntilWall(source, p => p + GridDirection.Right);
            FillUntilWall(source, p => p + GridDirection.Left);

            void FillUntilWall(Point2d source, Func<Point2d, Point2d> nextPoint)
            {
                var point = source;
                while (GetCellType(point) != CellType.Clay)
                {
                    _map[point] = CellType.StillWater;
                    point = nextPoint(point);
                }
            }
        }

        private bool InGrid(Point2d point2d) => gridDimensions.Contains(point2d);

        private CellType GetCellType(Point2d point) => _map.TryGetValue(point, out var ct)
            ? ct
            : CellType.Sand;

        public string Print() => Printer.Print(_map, gridDimensions);

        public void Write(MemoryStream stream) => stream.Write(Encoding.UTF8.GetBytes(Printer.Print(_map, gridDimensions)));
    }
}
