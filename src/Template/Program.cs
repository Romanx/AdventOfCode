using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using Humanizer;
using LibGit2Sharp;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;
using Zio;
using Zio.FileSystems;

namespace Template
{
    class Program
    {
        /// <param name="date">The date to generate the defaults for.</param>
        static async Task Main(string? date = null)
        {
            var challengeDate = string.IsNullOrWhiteSpace(date)
                ? SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate()
                : LocalDatePattern.Iso.Parse(date).Value;

            var templator = new Templator(challengeDate);
            await templator.Execute();
        }

        private class Templator : IDisposable
        {
            private readonly UPath projectRoot;
            private readonly Repository repo;
            private readonly LocalDate challengeDate;
            private readonly PhysicalFileSystem fs;

            public Templator(LocalDate date)
            {
                if (date.Month != 12)
                    throw new InvalidOperationException("Advent of code only runs in december!");

                fs = new PhysicalFileSystem();
                var path = fs.ConvertPathFromInternal(AppDomain.CurrentDomain.BaseDirectory);
                projectRoot = path / "../../../../../";
                repo = new Repository(fs.ConvertPathToInternal(projectRoot));
                challengeDate = date;
            }

            internal async Task Execute()
            {
                var url = new Uri($"https://adventofcode.com/{challengeDate.Year}/day/{challengeDate.Day}");
                var title = await GetChallengeTitle(url);

                var root = projectRoot / "src/years";
                var projectPath = WriteTemplate(title, root, fs);
                await AddProjectToSolution(projectPath);

                var signature = repo.Config.BuildSignature(DateTimeOffset.Now);
                repo.Commit($"{challengeDate.Year} Day {challengeDate.Day}: {title}", signature, signature);
            }

            static async Task<string> GetChallengeTitle(Uri challengeUrl)
            {
                using var client = new HttpClient();
                var body = await client.GetStringAsync(challengeUrl);
                var context = BrowsingContext.New(AngleSharp.Configuration.Default);
                var parser = context.GetService<IHtmlParser>();
                var document = parser.ParseDocument(body);

                var title = document.QuerySelector("h2")
                    .TextContent
                    .Trim('-', ' ');

                var split = title.Split(':', StringSplitOptions.TrimEntries);

                return split[1];
            }

            public void Dispose()
            {
                repo.Dispose();
            }

            string WriteTemplate(string title, UPath yearRoot, IFileSystem fs)
            {
                var dayWords = challengeDate.Day.ToWords();
                var yearPath = yearRoot / $"{challengeDate.Year}";
                var dayPath = yearPath / $"day-{dayWords}";

                fs.CreateDirectory(yearPath);
                fs.CreateDirectory(dayPath);

                var ns = $"Day{dayWords.Titleize().Replace(" ", "")}{challengeDate.Year}";
                var proj = $"{challengeDate.Day:00}_{ns}.csproj";

                fs.WriteAllText(dayPath / proj, ProjectTemplate.Render(new
                {
                    ChallengeNamespace = ns,
                }));
                Commands.Stage(repo, $"{fs.ConvertPathToInternal(dayPath / proj)}");

                fs.WriteAllText(dayPath / $"Challenge.cs", ChallengeTemplate.Render(new
                {
                    ChallengeNamespace = ns,
                    challengeDate.Year,
                    challengeDate.Day,
                    ChallengeTitle = title
                }));
                Commands.Stage(repo, $"{fs.ConvertPathToInternal(dayPath / $"Challenge.cs")}");

                return fs.ConvertPathToInternal(dayPath / proj);
            }

            async Task AddProjectToSolution(string projectPath)
            {
                var process = new Process();
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.WorkingDirectory = fs.ConvertPathToInternal(projectRoot);
                process.StartInfo.Arguments = $"sln add {projectPath}";

                process.Start();
                await process.WaitForExitAsync();
                Commands.Stage(repo, $"{fs.ConvertPathToInternal(projectRoot / $"AdventOfCode.sln")}");
            }

            private static Scriban.Template ProjectTemplate { get; } = Scriban.Template.Parse(@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <RootNamespace>{{challenge_namespace}}</RootNamespace>
  </PropertyGroup>
</Project>".Trim());

            private static Scriban.Template ChallengeTemplate { get; } = Scriban.Template.Parse(@"
namespace {{challenge_namespace}};

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate({{year}}, 12, {{day}}), ""{{challenge_title}}"");

    public void PartOne(IInput input, IOutput output)
    {
    }

    public void PartTwo(IInput input, IOutput output)
    {
    }
}

internal static class ParseExtensions
{
}
".Trim());
        }
    }
}
