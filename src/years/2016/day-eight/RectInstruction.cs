using Shared;
using Shared.Grid;

namespace DayEight2016
{
    record RectInstruction(int Width, int Height) : Instruction(CommandType.Rect)
    {
        public override Display Apply(Display display)
        {
            var builder = display.DisplayState.ToBuilder();
            var area = Area2d.Create(Point2d.Origin, new Point2d(Width - 1, Height - 1));

            foreach (var point in area.Items)
            {
                builder[point] = LightState.On;
            }

            return display with
            {
                DisplayState = builder.ToImmutable()
            };
        }
    }
}
