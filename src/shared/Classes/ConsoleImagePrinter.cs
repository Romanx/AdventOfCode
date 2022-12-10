using Shared.Grid;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace Shared
{
    public sealed class ConsoleImagePrinter : GridScanner
    {
        private readonly Color[,] array;
        private readonly Canvas canvas;

        private ConsoleImagePrinter(Color[,] array)
            : base(Area2d.Create(array))
        {
            this.array = array;
            canvas = new Canvas(this.array.GetLength(1), this.array.GetLength(0));
        }

        public static Canvas Print(Color[,] array)
        {
            var printer = new ConsoleImagePrinter(array);
            printer.Scan();
            return printer.canvas;
        }

        public override void OnEndOfRow() {}

        public override void OnPosition(Point2d point)
        {
            canvas.SetPixel(point.X, point.Y, array[point.Y, point.X]);
        }
    }
}
