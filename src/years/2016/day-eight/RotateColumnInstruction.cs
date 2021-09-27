using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DayEight2016
{
    record RotateColumnInstruction(int Column, int ShiftBy) : Instruction(CommandType.RotateColumn)
    {
        public override Display Apply(Display display)
        {
            var offset = new Point2d(0, ShiftBy);

            var on = display.DisplayState
                .Where(kvp => kvp.Value is LightState.On && kvp.Key.X == Column)
                .Select(kvp =>
                {
                    var newPosition = kvp.Key + offset;

                    if (display.DisplayArea.Contains(newPosition) is false)
                    {
                        newPosition = new Point2d(Column, (kvp.Key.Y + ShiftBy) % display.DisplayArea.Height - 1);
                    }

                    return KeyValuePair.Create(newPosition, kvp.Value);
                });

            var nextState = display.DisplayState.ToBuilder();

            ClearColumn(display, Column, nextState);
            SetActiveLights(on, nextState);

            return display with
            {
                DisplayState = nextState.ToImmutable()
            };

            static void ClearColumn(
                Display display,
                int column,
                ImmutableDictionary<Point2d, LightState>.Builder builder)
            {
                for (var y = 0; y < display.DisplayArea.Height; y++)
                {
                    var point = new Point2d(column, y);
                    builder[point] = LightState.Off;
                }
            }

            static void SetActiveLights(
                IEnumerable<KeyValuePair<Point2d, LightState>> activeLights,
                ImmutableDictionary<Point2d, LightState>.Builder builder)
            {
                foreach (var (key, value) in activeLights)
                {
                    builder[key] = value;
                }
            }
        }
    }
}
