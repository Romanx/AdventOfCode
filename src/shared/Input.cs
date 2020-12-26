using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NodaTime;
using NodaTime.Text;
using Shared.Helpers;
using Zio;

namespace Shared
{
    public class Input : IInput
    {
        private readonly FileEntry _inputFile;
        private string? _input;

        public Input(DirectoryEntry inputDirectory, LocalDate date)
        {
            foreach (var file in inputDirectory.EnumerateFiles(searchOption: SearchOption.AllDirectories))
            {
                if (IsCorrectInputFile(file, date))
                {
                    _inputFile = file;
                    return;
                }
            }

            throw new InvalidOperationException($"Unable to find input file for date {LocalDatePattern.Iso.Format(date)}");

            static bool IsCorrectInputFile(FileEntry file, LocalDate date)
            {
                var isoParsed = LocalDatePattern.Iso.Parse(file.NameWithoutExtension);
                var yearDayParsed = LocalDatePattern.Create("uuuu-dd", CultureInfo.InvariantCulture, date).Parse(file.NameWithoutExtension);

                return file.NameWithoutExtension.Equals($"day-{date.Day}", StringComparison.OrdinalIgnoreCase) ||
                       file.NameWithoutExtension.Equals($"day-{date.Year}-{date.Day}") ||
                       isoParsed.Success && isoParsed.Value == date ||
                       yearDayParsed.Success && yearDayParsed.Value == date;
            }
        }

        public char[,] As2DArray()
        {
            var lines = AsLines().ToArray();
            var array = new char[lines[0].Length, lines.Length];

            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y].Span;
                for (var x = 0; x < line.Length; x++)
                {
                    array[x, y] = line[x];
                }
            }

            return array;
        }

        public IEnumerable<ReadOnlyMemory<char>> AsLines()
        {
            foreach (var line in AsStringLines())
                yield return line.AsMemory();
        }

        public ReadOnlyMemory<ReadOnlyMemory<char>>[] AsParagraphs()
        {
            var lines = AsLines().ToArray().AsMemory();

            return SpanHelpers.SplitByBlankLines(lines)
                .ToArray();
        }

        public ReadOnlyMemory<char> AsReadOnlyMemory() => ReadAndCacheInput().AsMemory();

        public string AsString() => ReadAndCacheInput();

        public IEnumerable<string> AsStringLines()
        {
            var @in = ReadAndCacheInput();

            using var reader = new StringReader(@in);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private string ReadAndCacheInput()
        {
            if (_input is null)
            {
                _input = _inputFile.ReadAllText(Encoding.UTF8);
                return _input;
            }

            return _input;
        }
    }
}
