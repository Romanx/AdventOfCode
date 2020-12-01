using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NodaTime;
using Shared;
using Spectre.Console;
using Zio;
using Zio.FileSystems;

namespace Runner
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (HasSelectedPosition(args, out var position))
            {
                await RunChallenge(position);
                return;
            }

            AnsiConsole.Render(new FigletText("Advent of Code!").Centered().Color(Color.Red));

            var index = 1;
            foreach (var group in Challenges.GroupBy(c => c.Info.Date.Year).OrderBy(c => c.Key))
            {
                AnsiConsole.Render(new Rule(group.Key.ToString()) { Style = new Style(foreground: Color.Gold1) }.LeftAligned());

                foreach (var challenge in group)
                {
                    AnsiConsole.MarkupLine("[#00d7ff]({0}) {1}:[/] {2}", index, challenge.Info.Date, challenge.Info.Name);
                    index++;
                }
            }

            AnsiConsole.Render(new Rule() { Style = new Style(foreground: Color.Gold1) }.LeftAligned());

            var selectedIndex = AnsiConsole.Ask<int>("What challenge would you like to run?");
            await RunChallenge(selectedIndex);

            static bool HasSelectedPosition(string[] args, out int position)
            {
                if (args.Length == 1)
                {
                    return int.TryParse(args[0], out position);
                }

                position = 0;
                return false;
            }
        }

        public static async Task RunChallenge(int position)
        {
            var index = position - 1;
            if (index < 0 || index > Challenges.Length)
            {
                throw new IndexOutOfRangeException($"Challenge does not exist at position: '{position}'");
            }

            var challenge = Challenges[index];
            Console.Clear();
            AnsiConsole.Render(new FigletText("Advent of Code!").Centered().Color(Color.Red));
            AnsiConsole.Render(new Markup($"[bold #00d7ff]{challenge.Info.Date}: {challenge.Info.Name}[/]").Centered());

            var fs = new PhysicalFileSystem();
            var basePath = fs.ConvertPathFromInternal(AppDomain.CurrentDomain.BaseDirectory);
            var root = basePath / "../../../../../";
            var inputPath = root / "inputs";
            var outputPath = root / "outputs";

            var dayOutputDirectory = DayOutputDirectory(challenge.Info.Date, outputPath, fs);
            var input = new Input(new DirectoryEntry(fs, inputPath), challenge.Info.Date);

            var partOneOutput = new Output(dayOutputDirectory, fs);
            challenge.PartOne(input, partOneOutput);
            await WriteOutputs("Part 1", partOneOutput, fs);

            Console.WriteLine();

            var partTwoOutput = new Output(dayOutputDirectory, fs);
            challenge.PartTwo(input, partTwoOutput);
            await WriteOutputs("Part 2", partTwoOutput, fs);

            static UPath DayOutputDirectory(LocalDate date, UPath outputPath, FileSystem fs)
            {
                var dir = outputPath / $"{date.Year}-{date.Month}-{date.Day}";

                if (fs.DirectoryExists(dir))
                {
                    fs.DeleteDirectory(dir, true);
                }

                return dir;
            }
        }

        private static async Task WriteOutputs(string header, Output output, PhysicalFileSystem fs)
        {
            AnsiConsole.Render(new Rule(header) { Style = new Style(foreground: Color.Gold1) }.LeftAligned());

            var props = output.GetProperties();

            if (props.Length > 0)
            {
                var table = new Table()
                    .Title("Properties", new Style(foreground: Color.DeepSkyBlue1))
                    .Border(TableBorder.Square)
                    .LeftAligned()
                    .AddColumns("", "")
                    .HideHeaders();

                foreach (var (name, value) in props)
                {
                    table.AddRow(new Markup($"[dodgerblue1]{name}[/]"), new Text(value));
                }

                AnsiConsole.Render(table);
            }

            var paths = await output.OutputFiles();
            if (paths.Length > 0)
            {
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .LeftAligned()
                    .AddColumns(new TableColumn($"[deepskyblue1]Paths[/]"));

                foreach (var path in paths)
                {
                    var converted = fs.ConvertPathToInternal(path);

                    table.AddRow(new Markup(converted));
                }

                AnsiConsole.Render(table);
            }
        }

        private static ImmutableArray<Challenge> Challenges { get; } = Runner.Challenges.BuildChallenges();
    }
}
