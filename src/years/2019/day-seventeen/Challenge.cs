using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shared2019;
using Spectre.Console;
using static Shared.Helpers.PointHelpers;

namespace DaySeventeen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 17), "Set and Forget");

    public async Task PartOne(IInput input, IOutput output)
    {
        var map = await ScaffoldingInterface.ScanScaffolding(input.AsIntcodeProgram());

        var scaffolding = map
            .Where(kvp => kvp.Value is not '.')
            .Select(kvp => kvp.Key)
            .ToImmutableHashSet();
        var intersections = Intersections(scaffolding);

        var alignment = intersections
            .Sum(intersection => intersection.Center.X * intersection.Center.Y);

        output.WriteProperty("Alignment Sum", alignment);

        var printed = GridPrinter.Print(map);

        output.WriteBlock(() =>
        {
            return new Panel(printed);
        });
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var scaffoldProgram = input.AsIntcodeProgram();
        var map = await ScaffoldingInterface.ScanScaffolding(scaffoldProgram);
        var path = BuildPath(map);

        var (robotProgram, functions) = CalculatePathFunctions(path);

        scaffoldProgram = scaffoldProgram.SetItem(0, 2);
        var dust = await ScaffoldingInterface.MoveRobot(scaffoldProgram, robotProgram, functions);

        output.WriteProperty("Amount of dust retrieved", dust);
    }

    private static (string Program, ImmutableArray<string[]> Functions) CalculatePathFunctions(ImmutableArray<string> path)
    {
        var functions = ImmutableArray.CreateBuilder<string[]>(3);

        string symbols;
        while (true)
        {
            ReadOnlySpan<string> scratch = path.ToArray();
            for (var i = 0; i < 3; i++)
            {
                var len = Random.Shared.Next(1, 9);
                var pinnedLength = len > scratch.Length
                    ? scratch.Length
                    : len;

                if (pinnedLength is 0)
                    break;

                var sym = scratch[0..pinnedLength];
                scratch = scratch.RemoveSequence(sym);
                functions.Add(sym.ToArray());
            }

            symbols = ConvertToSymbols(path, functions);

            if (Regex.IsMatch(symbols, "^[ABC]+$"))
            {
                break;
            }

            functions.Clear();
        }

        return (symbols, functions.MoveToImmutable());

        static string ConvertToSymbols(ImmutableArray<string> path, ImmutableArray<string[]>.Builder groups)
        {
            ReadOnlySpan<string> @out = path.ToArray();
            for (var i = 0; i < groups.Count; i++)
            {
                var @char = (char)('A' + i);
                ReadOnlySpan<string> function = groups[i];

                @out = @out.ReplaceSequence(function, new[] { $"{@char}" });
            }

            return string.Join("", @out.ToArray());
        }
    }

    private static ImmutableArray<string> BuildPath(ImmutableDictionary<Point2d, char> map)
    {
        var scaffolding = map
            .Where(kvp => kvp.Value is not '.')
            .Select(kvp => kvp.Key)
            .ToImmutableHashSet();

        var output = ImmutableArray.CreateBuilder<string>();
        var robot = map.First(k => k.Value == '^').Key;

        var position = robot;
        var direction = Direction.North;
        char orientation = 'U';
        var count = 1;

        while (true)
        {
            var adjacent = GetNeighbours(position, scaffolding, AdjacencyType.Cardinal);

            if (adjacent[direction] is Point2d forwards)
            {
                count++;
                position = forwards;
            }
            else if (adjacent[direction.Left()] is Point2d left)
            {
                if (count > 1)
                {
                    output.Add($"{orientation}");
                    output.Add($"{count}");
                }

                orientation = 'L';
                direction = direction.Left();
                count = 1;
                position = left;
            }
            else if (adjacent[direction.Right()] is Point2d right)
            {
                if (count > 1)
                {
                    output.Add($"{orientation}");
                    output.Add($"{count}");
                }
                orientation = 'R';
                direction = direction.Right();
                count = 1;
                position = right;
            }
            else
            {
                if (count > 1)
                {
                    output.Add($"{orientation}");
                    output.Add($"{count}");
                }
                break;
            }
        }

        return output.ToImmutable();
    }
}
