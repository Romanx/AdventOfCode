using System;
using Humanizer;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using Zio;
using Zio.FileSystems;

namespace Template
{
    class Program
    {
        static void Main(string[] args)
        {
            var date = args.Length == 1
                ? LocalDatePattern.Iso.Parse(args[0]).Value
                : SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate();

            var fs = new PhysicalFileSystem();
            var path = fs.ConvertPathFromInternal(AppDomain.CurrentDomain.BaseDirectory);
            var root = path / "../../../../../src/years";

            WriteTemplate(date, root, fs);
        }

        static void WriteTemplate(LocalDate date, UPath yearRoot, IFileSystem fs)
        {
            var dayWords = date.Day.ToWords();
            var yearPath = yearRoot / $"{date.Year}";
            var dayPath = yearPath / $"day-{dayWords}";

            fs.CreateDirectory(yearPath);
            fs.CreateDirectory(dayPath);

            var ns = $"Day{dayWords.Titleize().Replace(" ", "")}{date.Year}";

            fs.WriteAllText(dayPath / $"{ns}.csproj", @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
</Project>".Trim());

            fs.WriteAllText(dayPath / $"Challenge.cs", @$"
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace {ns}
{{
    public class Challenge : Shared.Challenge
    {{
        public override ChallengeInfo Info {{ get; }} = new ChallengeInfo(new LocalDate({date.Year}, {date.Month}, {date.Day}), """");

        public override void PartOne(IInput input, IOutput output)
        {{
        }}

        public override void PartTwo(IInput input, IOutput output)
        {{
        }}
    }}

    internal static class ParseExtensions
    {{
    }}
}}
".TrimStart());
        }
    }
}
