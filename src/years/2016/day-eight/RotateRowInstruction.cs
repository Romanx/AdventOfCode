using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DayEight2016
{
    record RotateRowInstruction(int Row, int ShiftBy) : Instruction(CommandType.RotateRow)
    {
        public override Display Apply(Display display)
        {
            var offset = new Point2d(ShiftBy, 0);

            var on = display.DisplayState
                .Where(kvp => kvp.Value is LightState.On && kvp.Key.Y == Row)
                .Select(kvp =>
                {
                    var newPosition = kvp.Key + offset;

                    if (display.DisplayArea.Contains(newPosition) is false)
                    {
                        newPosition = new Point2d(
                            (kvp.Key.X + ShiftBy) % display.DisplayArea.Width - 1,
                            Row);
                    }

                    return KeyValuePair.Create(newPosition, kvp.Value);
                });

            var nextState = display.DisplayState.ToBuilder();

            ClearRow(display, Row, nextState);
            SetActiveLights(on, nextState);

            return display with
            {
                DisplayState = nextState.ToImmutable()
            };

            static void ClearRow(
                Display display,
                int row,
                ImmutableDictionary<Point2d, LightState>.Builder builder)
            {
                for (var x = 0; x < display.DisplayArea.Width; x++)
                {
                    var point = new Point2d(x, row);
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
