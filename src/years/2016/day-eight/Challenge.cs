using System.IO;
using Shared.Grid;
using SixLabors.ImageSharp;
using Spectre.Console;
using Color = SixLabors.ImageSharp.Color;

namespace DayEight2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 8), "Two-Factor Authentication");

        public override void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var display = Display.Create(50, 6)
                .ApplyInstructions(instructions);

            output.WriteProperty("Lit pixles", display.DisplayState
                .Count(kvp => kvp.Value is LightState.On));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var display = Display.Create(50, 6)
                .ApplyInstructions(instructions);

            output.WriteBlock(() => display.Image());
        }
    }

    record Display(Area2d DisplayArea, ImmutableDictionary<Point2d, LightState> DisplayState)
    {
        public static Display Create(int width, int height)
        {
            var area = Area2d.Create(Point2d.Origin, new Point2d(width - 1, height - 1));
            var state = area.Items
                .ToImmutableDictionary(k => k, v => LightState.Off);

            return new Display(area, state);
        }

        public Display ApplyInstructions(ImmutableArray<Instruction> instructions)
        {
            return instructions
                .Aggregate(this, static (agg, instruction) => instruction.Apply(agg));
        }

        public string Print() => GridPrinter.Print(DisplayState);

        public CanvasImage Image()
        {
            var writer = new ImageWriter(DisplayState);
            var stream = new MemoryStream();
            var image = writer.Generate();
            image.SaveAsPng(stream);
            stream.Flush();
            stream.Position = 0;

            return new CanvasImage(stream);
        }

        private sealed class ImageWriter : GridImageWriter<LightState>
        {
            public ImageWriter(IReadOnlyDictionary<Point2d, LightState> map) : base(map)
            {
            }

            protected override Color GetColorForPoint(Point2d point)
            {
                var cellType = _map[point];

                return cellType switch
                {
                    LightState.Off => Color.Black,
                    LightState.On => Color.Yellow,
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Instruction> Parse(this IInput input)
        {
            var rectRegex = new PcreRegex("rect ([0-9]+)x([0-9]+)");
            var rotateRegex = new PcreRegex("rotate (?<Type>.*) (?>x|y)=([0-9]+) by ([0-9]+)");

            var builder = ImmutableArray.CreateBuilder<Instruction>();

            foreach (var line in input.Lines.AsMemory())
            {
                if (line.Span.StartsWith("rect"))
                {
                    var match = rectRegex.Match(line.Span);

                    builder.Add(new RectInstruction(
                        int.Parse(match[1].Value),
                        int.Parse(match[2].Value)));
                }
                else
                {
                    var match = rotateRegex.Match(line.Span);

                    var isRow = match["Type"].Value.Equals("row", StringComparison.Ordinal);
                    var index = int.Parse(match[2].Value);
                    var shift = int.Parse(match[3].Value);

                    Instruction command = isRow
                        ? new RotateRowInstruction(index, shift)
                        : new RotateColumnInstruction(index, shift);

                    builder.Add(command);
                }
            }

            return builder.ToImmutableArray();
        }
    }
}
