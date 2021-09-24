using System.Collections.Immutable;
using System.Linq;
using Microsoft.Toolkit.HighPerformance;
using Shared;

namespace DayTwo2016
{
    class Keypad
    {
        private readonly ImmutableDictionary<Point2d, char> _keypad;

        public Keypad(char[,] pad)
        {
            var span = pad.AsSpan2D();

            var builder = ImmutableDictionary.CreateBuilder<Point2d, char>();
            for (var y = 0; y < span.Height; y++)
            {
                var row = span.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    if (row[x] != ' ')
                    {
                        builder.Add(new Point2d(x, y), row[x]);
                    }
                }
            }

            _keypad = builder.ToImmutable();
            FiveKey = _keypad.Where(kvp => kvp.Value == '5').First().Key;
        }

        public bool Valid(Point2d point) => _keypad.ContainsKey(point);

        public Point2d FiveKey { get; }

        public char Key(Point2d point) => _keypad[point];
    }
}
