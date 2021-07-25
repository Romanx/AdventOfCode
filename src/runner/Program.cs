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
            var console = AnsiConsole.Console;
            if (HasSelectedPosition(args, out var position))
            {
                await RunChallenge(console, position);
                return;
            }

            WriteMenu(console);
            var challengeRange = 1..Challenges.Length;

            var prompt = new TextPrompt<int>("What challenge would you like to run?")
                .DefaultValue(Challenges.Length)
                .Validate(index =>
                {
                    return challengeRange.Contains(index) is false
                        ? ValidationResult.Error($"[red]Index not in the range of[/] [aqua]{challengeRange.Start} to {challengeRange.End}[/]")
                        : ValidationResult.Success();
                });

            var selectedIndex = console.Prompt(prompt);
            try
            {
                await RunChallenge(console, selectedIndex);
            }
            catch (Exception ex)
            {
                console.WriteException(ex);
            }

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

        public static async Task RunChallenge(IAnsiConsole console, int position)
        {
            var index = position - 1;
            if (index < 0 || index > Challenges.Length)
            {
                throw new IndexOutOfRangeException($"Challenge does not exist at position: '{position}'");
            }

            var challenge = Challenges[index];
            ChallengeHeader(console, challenge);

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

            static void ChallengeHeader(IAnsiConsole console, Challenge challenge)
            {
                Console.Clear();
                console.Write(new FigletText("Advent of Code!").Centered().Color(Color.Red));
                console.Write(new Markup($"[bold #00d7ff]{challenge.Info.Date}: {challenge.Info.Name}[/]").Centered());
            }
        }

        private static void WriteMenu(IAnsiConsole console)
        {
            console.Write(new FigletText("Advent of Code!").Centered().Color(Color.Red));

            var index = 1;
            foreach (var group in Challenges.GroupBy(c => c.Info.Date.Year).OrderBy(c => c.Key))
            {
                console.Write(new Rule($"[[ {group.Key} ]]") { Style = new Style(foreground: Color.Gold1) }.LeftAligned());
                var table = new Table()
                    .AddColumn(new TableColumn("").RightAligned())
                    .AddColumns("")
                    .HideHeaders()
                    .MinimalBorder();

                var color = new Color(0, 215, 255);
                foreach (var challenge in group)
                {
                    table.AddRow(
                        new Markup($"{index}".PadLeft(2), new Style(foreground: color, decoration: Decoration.Bold)),
                        new Markup($"[#00d7ff]{challenge.Info.Date} - [/]{challenge.Info.Name}"));

                    index++;
                }

                console.Write(table);
            }

            console.Write(new Rule() { Style = new Style(foreground: Color.Gold1) }.LeftAligned());
        }

        private static async Task WriteOutputs(string header, Output output, PhysicalFileSystem fs)
        {
            AnsiConsole.Render(new Rule(header) { Style = new Style(foreground: Color.Gold1) }.LeftAligned());

            var props = output.GetProperties();

            if (props.Length > 0)
            {
                var table = new Table()
                    .Border(TableBorder.Square)
                    .LeftAligned()
                    .AddColumns("", "")
                    .HideHeaders();

                foreach (var (name, value) in props)
                {
                    table.AddRow(new Markup($"[#00d7ff]{name}[/]"), new Text(value));
                }

                AnsiConsole.Render(table);
            }

            output.WriteBlocks(AnsiConsole.Console);

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
