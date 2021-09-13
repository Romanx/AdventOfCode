using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console.Rendering;

namespace Shared
{
    public interface IOutput
    {
        MemoryStream File(string fileName);

        Image<Rgba32> Image(string imageName, int width, int height);

        void AddImage(string imageName, Image image);

        void WriteBlock(Func<IRenderable> blockAction);

        void WriteProperty(string name, object? value, IFormatProvider? formatProvider = null);
    }
}
