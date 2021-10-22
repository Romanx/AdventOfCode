using System;
using System.Globalization;
using System.IO;
using System.Text;
using NodaTime;
using NodaTime.Text;
using Zio;

namespace Shared
{
    public static class FileInput
    {
        public static IInput Build(DirectoryEntry inputDirectory, LocalDate date)
        {
            foreach (var file in inputDirectory.EnumerateFiles(searchOption: SearchOption.AllDirectories))
            {
                if (IsCorrectInputFile(file, date))
                {
                    return new Input(file.ReadAllText(Encoding.UTF8));
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
    }
}
