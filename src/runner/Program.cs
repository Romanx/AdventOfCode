using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using NodaTime;
using NodaTime.Text;
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
            var challengeRange = 1..Challenges.Count;

            var prompt = new TextPrompt<int>("What challenge would you like to run?")
                .DefaultValue(Challenges.Count)
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
                    if (int.TryParse(args[0], out position))
                    {
                        return true;
                    }
                    else if (LocalDatePattern.Iso.Parse(args[0]).TryGetValue(default, out var date))
                    {
                        var challengePosition = Challenges.FindIndex(c => c.Info.Date == date);
                        if (challengePosition != -1)
                        {
                            position = challengePosition + 1;
                            return true;
                        }
                    }
                }

                position = 0;
                return false;
            }
        }

        public static async Task RunChallenge(IAnsiConsole console, int position)
        {
            var index = position - 1;
            if (index < 0 || index > Challenges.Count)
            {
                throw new IndexOutOfRangeException($"Challenge does not exist at position: '{position}'");
            }

            var challenge = Challenges[index];

            var fs = new PhysicalFileSystem();
            var basePath = fs.ConvertPathFromInternal(AppDomain.CurrentDomain.BaseDirectory);
            var root = basePath / "../../../../../";
            var inputPath = root / "inputs";
            var outputPath = root / "outputs";

            var dayOutputDirectory = DayOutputDirectory(challenge.Info.Date, outputPath, fs);
            var input = FileInput.Build(new DirectoryEntry(fs, inputPath), challenge.Info.Date);

            await RunChallenge(
                console,
                challenge,
                fs,
                input,
                challenge =>
                {
                    return new Output(dayOutputDirectory, fs);
                });

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

        private static async Task WriteOutputs(
            IAnsiConsole console,
            string header,
            Output output,
            TimeSpan elapsed,
            IFileSystem fs)
        {
            console.Write(new Rule($"{header} ({elapsed.Humanize()})") { Style = new Style(foreground: Color.Gold1) }.LeftAligned());

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
                    table.AddRow(new Markup($"[#00d7ff]{Markup.Escape(name)}[/]"), new Text(value));
                }

                console.Write(table);
            }

            output.WriteBlocks(console);

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

                console.Write(table);
            }
        }

        private static async Task RunChallenge(
            IAnsiConsole console,
            Challenge challenge,
            IFileSystem fileSystem,
            IInput input,
            Func<Challenge, Output> buildOutput)
        {
            ChallengeHeader(console, challenge);
            var type = challenge.GetType();

            await TryExecutePart(
                console,
                challenge,
                type,
                1,
                fileSystem,
                input,
                buildOutput);

            await TryExecutePart(
                console,
                challenge,
                type,
                2,
                fileSystem,
                input,
                buildOutput);

            static async Task TryExecutePart(
                IAnsiConsole console,
                Challenge challenge,
                Type type,
                int methodNumber,
                IFileSystem fileSystem,
                IInput input,
                Func<Challenge, Output> buildOutput)
            {
                var methodName = $"Part{methodNumber.ToWords().Titleize()}";

                var method = GetPartMethod(type, methodName);

                if (method is not null)
                {
                    var output = buildOutput(challenge);
                    var returnType = method.ReturnType;
                    Stopwatch stopwatch;

                    if (returnType == typeof(Task))
                    {
                        stopwatch = Stopwatch.StartNew();
                        var task = (Task)method.Invoke(challenge, new object[] { input, output })!;
                        await task;
                    }
                    else
                    {
                        stopwatch = Stopwatch.StartNew();
                        method.Invoke(challenge, new object[] { input, output });
                    }
                    stopwatch.Stop();

                    await WriteOutputs(
                        console,
                        methodName.Humanize(LetterCasing.Title),
                        output,
                        stopwatch.Elapsed,
                        fileSystem);
                }
            }

            static void ChallengeHeader(IAnsiConsole console, Challenge challenge)
            {
                Console.Clear();
                console.Write(new FigletText("Advent of Code!").Centered().Color(Color.Red));
                console.Write(new Markup($"[bold #00d7ff]{challenge.Info.Date}: {challenge.Info.Name}[/]").Centered());
            }

            static MethodInfo? GetPartMethod(Type challengeType, string methodName)
            {
                return challengeType
                    .GetMethod(
                        methodName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Static,
                        new Type[] { typeof(IInput), typeof(IOutput) });
            }
        }

        private static ImmutableList<Challenge> Challenges { get; } = Runner.Challenges.BuildChallenges();
    }
}
