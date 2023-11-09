using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace RunnerGenerator;

[Generator]
public class ProgramGenerator : ISourceGenerator
{
    private static readonly SymbolDisplayFormat displayFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver 
        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        if (false && !Debugger.IsAttached)
        {
            Debugger.Launch();
        }

        var challenge = GetChallengeClass(
            context,
            receiver.ClassDeclarations);

        var details = BuildRunnerDetails(challenge);

        string source = """
            using System;
            using System.Threading.Tasks;
            using NodaTime;
            using NodaTime.Text;
            using Shared;
            using Spectre.Console;
            using Zio;
            using Zio.FileSystems;
            using Humanizer;
            using System.Diagnostics;

            public class ChallengeRunner
            {
                public static async Task Main(string[] args)
                {
                    var console = AnsiConsole.Console;
                    var challenge = new {{name}}();

                    var fs = new PhysicalFileSystem();
                    var basePath = fs.ConvertPathFromInternal(AppDomain.CurrentDomain.BaseDirectory);
                    var root = basePath / "../../../../../../../";
                    var inputPath = root / "inputs";
                    var outputPath = root / "outputs";

                    var dayOutputDirectory = DayOutputDirectory(challenge.Info.Date, outputPath, fs);
                    var input = FileInput.Build(new DirectoryEntry(fs, inputPath), challenge.Info.Date);

                    WriteHeader(console, challenge);

                    {{~ for part in parts ~}}
                    {
                        var output = new Output(dayOutputDirectory, fs);
                        var stopwatch = Stopwatch.StartNew();

                        {{ if part.isAsync }}
                            await challenge.{{part.name}}(input, output);
                        {{ else }}
                            challenge.{{part.name}}(input, output);
                        {{ end }}

                        stopwatch.Stop();

                        await WriteOutputs(
                            console,
                            "{{part.name}}".Humanize(LetterCasing.Title),
                            output,
                            stopwatch.Elapsed,
                            fs);
                    }
                    {{~ end ~}}
                }

                static void WriteHeader(IAnsiConsole console, Challenge challenge)
                {
                    console.Clear();
                    console.Write(new FigletText("Advent of Code!").Centered().Color(Color.Red));
                    console.Write(new Markup($"[bold #00d7ff]{challenge.Info.Date}: {challenge.Info.Name}[/]").Centered());
                }

                static UPath DayOutputDirectory(LocalDate date, UPath outputPath, FileSystem fs)
                {
                    var dir = outputPath / $"{date.Year}-{date.Month}-{date.Day}";

                    if (fs.DirectoryExists(dir))
                    {
                        fs.DeleteDirectory(dir, true);
                    }

                    return dir;
                }

                static async Task WriteOutputs(
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
            }
            """;

        var template = Template.Parse(source);

        var rendered = template.Render(details);

        context.AddSource("Program.g.cs", SourceText.From(rendered, Encoding.UTF8));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    private ChallengeRunnerDetails BuildRunnerDetails(ITypeSymbol challengeType)
    {
        var parts = challengeType.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(ms => ms.Name.StartsWith("Part"));

        return new()
        {
            Name = challengeType.ToDisplayString(displayFormat),
            Parts = MapParts(parts),
        };

        static ImmutableArray<PartMethod> MapParts(IEnumerable<IMethodSymbol> parts)
        {
            var builder = ImmutableArray.CreateBuilder<PartMethod>();
            foreach (var part in parts)
            {
                if (part.DeclaredAccessibility != Accessibility.Public)
                {
                    continue;
                }

                builder.Add(new PartMethod
                {
                    Name = part.Name,
                    IsAsync = !part.ReturnsVoid,
                });
            }

            return builder.ToImmutable();
        }
    }

    private ITypeSymbol GetChallengeClass(GeneratorExecutionContext context, IEnumerable<ClassDeclarationSyntax> classDeclarations)
    {
        var compilation = context.Compilation;

        var challenge = compilation.GetTypeByMetadataName("Shared.Challenge")!;
        foreach (var @class in classDeclarations)
        {
            var model = compilation.GetSemanticModel(@class.SyntaxTree);
            if (model.GetDeclaredSymbol(@class, context.CancellationToken) is ITypeSymbol symbol)
            {
                if (symbol.IsAbstract is false 
                    && symbol.DeclaredAccessibility == Accessibility.Public
                    && SymbolEqualityComparer.Default.Equals(symbol.BaseType, challenge))
                {
                    return symbol;
                }
            }
        }

        throw new InvalidOperationException("Could not find challenge class");
    }
}

public class ChallengeRunnerDetails
{
    public string Name { get; set; } = string.Empty;

    public ImmutableArray<PartMethod> Parts { get; set; } = ImmutableArray<PartMethod>.Empty;
};

public class PartMethod
{
    public string Name { get; set; } = string.Empty;

    public bool IsAsync { get; set; }
}

/// <summary>
/// Created on demand before each generation pass
/// </summary>
class SyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> ClassDeclarations { get; } = new List<ClassDeclarationSyntax>();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        // any field with at least one attribute is a candidate for property generation
        if (syntaxNode is ClassDeclarationSyntax decl)
        {
            ClassDeclarations.Add(decl);
        }
    }
}
