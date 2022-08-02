using static MoreLinq.Extensions.ToDelimitedStringExtension;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance;

namespace DayFourteen2018;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 14), "Chocolate Charts");

    public override void PartOne(IInput input, IOutput output)
    {
        var target = input.Content.AsInt();
        var result = GenerateRecipes(recipes => recipes.Count == target + 10)
            .TakeLast(10)
            .ToDelimitedString("");

        output.WriteProperty("Result", result);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var target = input.Content.CharactersToInt()
            .ToArray();

        var recipes = GenerateRecipes(recipes =>
        {
            var span = recipes.AsSpan();
            return span.EndsWith(target);
        });

        var result = recipes.Count - target.Length;

        output.WriteProperty("Result", result);
    }

    public List<int> GenerateRecipes(
        Func<List<int>, bool> stopCondition)
    {
        var history = new List<int> { 3, 7 };
        var elf1 = 0;
        var elf2 = 1;
        var shouldStop = false;

        while (shouldStop is false)
        {
            var next = history[elf1] + history[elf2];
            foreach (var value in AsDigits(next))
            {
                if (shouldStop is false)
                {
                    history.Add(value);
                    shouldStop = stopCondition(history);
                }
            }

            elf1 = (elf1 + history[elf1] + 1) % history.Count;
            elf2 = (elf2 + history[elf2] + 1) % history.Count;
        }

        return history;

        static IEnumerable<int> AsDigits(int number) =>
            number.ToString().ToCharArray().Select(@char => @char - '0');
    }
}
