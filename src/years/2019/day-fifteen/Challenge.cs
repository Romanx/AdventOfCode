using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Shared.Graph;
using Shared2019;
using Spectre.Console;

namespace DayFifteen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 15), "Oxygen System");

    public async Task PartOne(IInput input, IOutput output)
    {
        var droid = new Droid(input.AsIntcodeProgram());
        var map = await droid.Run();

        var graph = new SimpleGridGraph<CellType>(map, (_, _, _, cellType) => cellType is not CellType.Wall);
        var target = map.First(m => m.Value is CellType.OxygenSystem).Key;

        var path = Algorithms.BreadthFirstSearch(graph, Point2d.Origin, target);

        output.WriteProperty("Number of steps to Oxygen System", path.Length - 1);

        var updated = map.SetItems(path.Select(point => KeyValuePair.Create(point, CellType.Path)));
        var printed = GridPrinter.Print(updated);

        output.WriteBlock(() =>
        {
            return new Panel(printed);
        });
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var droid = new Droid(input.AsIntcodeProgram());
        var map = await droid.Run();

        var graph = new SimpleGridGraph<CellType>(map, (_, _, _, cellType) => cellType is not CellType.Wall);
        var target = map.First(m => m.Value is CellType.OxygenSystem).Key;

        var points = Algorithms.FloodFillWithSteps(graph, target, null);
        var maxSteps = points.Values.Max();

        output.WriteProperty("Number of minutes", maxSteps);
    }
}

public enum CellType
{
    [Display(Name = "#")]
    Wall = 0,
    [Display(Name = " ")]
    Empty = 1,
    [Display(Name = "0")]
    OxygenSystem = 2,
    [Display(Name = "D")]
    Path
}
