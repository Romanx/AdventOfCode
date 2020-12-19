using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Zio;

namespace Shared
{
    public class Output : IOutput
    {
        private readonly UPath _outputDirectory;
        private readonly IFileSystem _fileSystem;
        private readonly List<(string Key, string Value)> _values = new();
        private readonly Dictionary<string, Image<Rgba32>> _images = new Dictionary<string, Image<Rgba32>>();
        private readonly Dictionary<string, MemoryStream> _files = new Dictionary<string, MemoryStream>();

        public Output(UPath outputDirectory, IFileSystem fileSystem)
        {
            _outputDirectory = outputDirectory;
            _fileSystem = fileSystem;
        }

        public MemoryStream File(string fileName)
        {
            var file = new MemoryStream();
            _files.Add(fileName, file);
            return file;
        }

        public Image<Rgba32> Image(string imageName, int width, int height)
        {
            var image = new Image<Rgba32>(width, height);
            _images.Add(imageName, image);
            return image;
        }

        public void WriteProperty(string name, object? value, IFormatProvider? formatProvider = null)
        {
            if (value is null)
            {
                _values.Add((name, string.Empty));
            }
            else if (value is string str)
            {
                _values.Add((name, str));
            }
            else
            {
                var objStr = Convert.ToString(value, formatProvider);
                Debug.Assert(objStr is not null, "Should be able to turn object into a string");
                _values.Add((name, objStr));
            }
        }

        public ImmutableArray<(string Name, string Value)> GetProperties() => _values.ToImmutableArray();

        public async Task<ImmutableArray<string>> OutputFiles()
        {
            List<string> paths = new List<string>(_images.Count + _files.Count);

            foreach (var (fileName, image) in _images)
            {
                var (stream, entry) = GetFileOutputStream($"{fileName}.png");

                using (stream)
                {
                    await image.SaveAsPngAsync(stream);
                    paths.Add(entry.Path.FullName);
                }
            }

            foreach (var (fileName, file) in _files)
            {
                var (stream, entry) = GetFileOutputStream($"{fileName}");

                using (stream)
                {
                    file.Position = 0;
                    await file.CopyToAsync(stream);
                    paths.Add(entry.Path.FullName);
                    await stream.FlushAsync();
                }
            }

            return paths.ToImmutableArray();
        }

        private (Stream stream, FileEntry entry) GetFileOutputStream(string fileNameAndExtension)
        {
            var path = _outputDirectory / fileNameAndExtension;
            var fileEntry = new FileEntry(_fileSystem, path);
            _fileSystem.CreateDirectory(_outputDirectory);

            return (fileEntry.Open(FileMode.CreateNew, FileAccess.ReadWrite), fileEntry);
        }
    }
}
