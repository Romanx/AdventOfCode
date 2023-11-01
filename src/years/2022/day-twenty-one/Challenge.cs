using System.Collections.Frozen;

namespace DayTwentyOne2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 21), "Monkey Math");

    public void PartOne(IInput input, IOutput output)
    {
        var monkeys = input.ParseMonkeys();
        var root = monkeys["root"];

        output.WriteProperty("Root Monkey Value", root.Yell());
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var monkeys = input.ParseMonkeys();
        var root = monkeys["root"];
        var humanValue = root.CalculateHumanValue();

        output.WriteProperty("Human Value", humanValue);
    }
}

internal static class ParseExtensions
{
    public static FrozenDictionary<string, Monkey> ParseMonkeys(this IInput input)
    {
        var resolved = new Dictionary<string, Monkey>();
        var outstandingMonkies = new Queue<OutstandingMonkey>();

        foreach (var line in input.Lines.AsMemory())
        {
            var span = line.Span;
            var name = new string(span[0..4]);

            var action = span[6..];
            if (char.IsAsciiDigit(action[0]))
            {
                var number = long.Parse(action);
                resolved.Add(name, new NumberMonkey(name, number));
            }
            else
            {
                var op = action.IndexOfAny(['+', '-', '*', '/']);
                var argA = action[..op].Trim();
                var argB = action[(op + 1)..].Trim();

                outstandingMonkies.Enqueue(new OutstandingMonkey(
                    name,
                    new string(argA),
                    action[op],
                    new string(argB)));
            }
        }

        while (outstandingMonkies.TryDequeue(out var outstanding))
        {
            if (resolved.ContainsKey(outstanding.Name))
                continue;

            if (resolved.TryGetValue(outstanding.Arg1, out var left) &&
                resolved.TryGetValue(outstanding.Arg2, out var right))
            {
                resolved.Add(outstanding.Name, new FormulaMonkey(
                    outstanding.Name,
                    left,
                    right,
                    outstanding.Operator));
            }

            outstandingMonkies.Enqueue(outstanding);
        }

        return resolved.ToFrozenDictionary();
    }

    private record OutstandingMonkey(
        string Name,
        string Arg1,
        char Operator,
        string Arg2);
}
