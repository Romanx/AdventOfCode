using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console;

namespace Shared
{
    public static class OutputExtensions
    {
        public static IOutput WriteTable(
            this IOutput output,
            IDictionary<string, string> dictionary)
        {
            output.WriteBlock(() =>
            {
                var table = new Table()
                {
                    ShowHeaders = false
                };
                table.AddColumns("", "");
                foreach (var (key, value) in dictionary)
                {
                    table.AddRow(key, value);
                }

                return table;
            });

            return output;
        }

        public static IOutput WriteImage(
            this IOutput output,
            Image image)
        {
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            output.WriteBlock(() =>
            {
                return new CanvasImage(stream);
            });

            return output;
        }

        public static void AddPanel(this IOutput output, string title, string content)
        {
            output.WriteBlock(() =>
            {
                return new Panel(new Text(content))
                {
                    Header = new PanelHeader(title, Justify.Center)
                };
            });
        }
    }
}
