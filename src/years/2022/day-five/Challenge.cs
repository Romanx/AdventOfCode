using System.Text.RegularExpressions;

namespace DayFive2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 5), "Supply Stacks");

    public void PartOne(IInput input, IOutput output)
    {
        var (stacks, procedures) = input.Parse();

        foreach (var procedure in procedures)
        {
            Apply(stacks, procedure);
        }

        output.WriteProperty("Top of Stacks", ReadTopOfStacks(stacks));

        static void Apply(Stack<char>[] stacks, Procedure procedure)
        {
            var source = stacks[procedure.Source - 1];
            var target = stacks[procedure.Target - 1];

            for (var i = 0; i < procedure.Count; i++)
            {
                var item = source.Pop();
                target.Push(item);
            }
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (stacks, procedures) = input.Parse();

        foreach (var procedure in procedures)
        {
            Apply(stacks, procedure);
        }

        output.WriteProperty("Top of Stacks", ReadTopOfStacks(stacks));

        static void Apply(Stack<char>[] stacks, Procedure procedure)
        {
            var source = stacks[procedure.Source - 1];
            var target = stacks[procedure.Target - 1];
            var queue = new Stack<char>((int)procedure.Count);

            for (var i = 0; i < procedure.Count; i++)
            {
                var item = source.Pop();
                queue.Push(item);
            }

            while (queue.TryPop(out var item))
            {
                target.Push(item);
            }
        }
    }

    private string ReadTopOfStacks(Stack<char>[] stacks)
    {
        Span<char> result = stackalloc char[stacks.Length];
        for (var i = 0; i < stacks.Length; i++)
        {
            result[i] = stacks[i].Peek();
        }

        return new string(result);
    }
}

readonly record struct Procedure(uint Count, uint Source, uint Target);
