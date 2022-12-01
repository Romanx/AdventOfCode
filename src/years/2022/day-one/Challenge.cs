namespace DayOne2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 1), "Calorie Counting");

    public void PartOne(IInput input, IOutput output)
    {
        var elves = input.ParseElves();

        var elfWithTheMost = elves.MaxBy(e => e.TotalCalories);

        output.WriteProperty("Elf with the Most Calories", elfWithTheMost);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var elves = input.ParseElves();
        var caloriesOnTop3 = elves
            .OrderByDescending(e => e.TotalCalories)
            .Take(3)
            .Sum(e => e.TotalCalories);

        output.WriteProperty("Total Calories on Top 3", caloriesOnTop3);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Elf> ParseElves(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var builder = ImmutableArray.CreateBuilder<Elf>(paragraphs.Length);

        foreach (var block in paragraphs)
        {
            var total = 0;
            for (var i = 0; i < block.Length; i++)
            {
                total += int.Parse(block.Span[i].Span, null);
            }

            builder.Add(new Elf(total));
        }

        return builder.MoveToImmutable();
    }
}

readonly record struct Elf(int TotalCalories)
{
    public override string ToString()
        => $$"""Elf { Food = {{TotalCalories}} }""";
}
