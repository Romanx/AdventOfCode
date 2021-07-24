using System;
using System.Collections.Generic;
using System.Text;
using Shared;

namespace DaySeventeen2018
{
    public static class Printer
    {
        public static string Print(IReadOnlyDictionary<Point2d, CellType> map, GridDimensions dimensions)
        {
            var printer = new GridPrinter(map, dimensions);
            printer.Scan();
            return printer.AsString();
        }

        private class GridPrinter : GridScanner
        {
            private readonly StringBuilder _builder;
            private readonly IReadOnlyDictionary<Point2d, CellType> _map;

            public GridPrinter(IReadOnlyDictionary<Point2d, CellType> map, GridDimensions dimensions)
                : base(dimensions)
            {
                _builder = new StringBuilder();
                _map = map;
            }

            public string AsString() => _builder.ToString();

            public override void OnPosition(Point2d point)
            {
                var cellType = _map.TryGetValue(point, out var ct)
                        ? ct
                        : CellType.Sand;

                _builder.Append(cellType switch
                {
                    CellType.Sand => '.',
                    CellType.Spring => '+',
                    CellType.FlowingWater => '|',
                    CellType.StillWater => '~',
                    CellType.Clay => '#',
                    _ => throw new NotImplementedException(),
                });
            }

            public override void OnEndOfRow() => _builder.AppendLine();
        }
    }
}
