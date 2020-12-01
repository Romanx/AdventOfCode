using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Shared
{
    public interface IOutput
    {
        MemoryStream File(string fileName);

        Image<Rgba32> Image(string imageName, int width, int height);

        void WriteProperty(string name, string value);
    }
}
