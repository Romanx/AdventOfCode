using Spectre.Console;

namespace DayTwentyThree2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 23), "Unstable Diffusion");

    public void PartOne(IInput input, IOutput output)
    {
        var elves = input.ParseElves();
        var simulation = new Simulation(elves);

        for (var i = 0; i < 10; i++)
        {
            simulation.Round();
        }

        output.WriteProperty("Number of Empty Spaces", simulation.EmptySpaces);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var elves = input.ParseElves();
        var simulation = new Simulation(elves);
        bool anyMoved;

        do
        {
            anyMoved = simulation.Round();
        } while (anyMoved);

        output.WriteProperty("Round number of no moves", simulation.RoundNumber);
    }
}

internal static class ParseExtensions
{
    public static ImmutableHashSet<Point2d> ParseElves(this IInput input)
    {
        return input.As2DPoints()
            .Where(x => x.Character is '#')
            .Select(x => x.Point)
            .ToImmutableHashSet();
    }
}

