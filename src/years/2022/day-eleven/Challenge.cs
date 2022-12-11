using System.Numerics;
using CommunityToolkit.HighPerformance;
using Spectre.Console;

namespace DayEleven2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 11), "Monkey in the Middle");

    public void PartOne(IInput input, IOutput output)
    {
        var monkeys = input.ParseMonkeys();

        var rounds = 20;

        for (var i = 0; i < rounds; i++)
        {
            MonkeyGame.RunRound(monkeys, keepingCalm: true);
        }

        output.WriteProperty("Total Monkey Business", CalculateMonkeyBusiness(monkeys));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var monkeys = input.ParseMonkeys();

        var rounds = 10_000;

        for (var i = 0; i < rounds; i++)
        {
            MonkeyGame.RunRound(monkeys, keepingCalm: false);
        }

        output.WriteProperty("Total Monkey Business", CalculateMonkeyBusiness(monkeys));
    }

    private long CalculateMonkeyBusiness(IEnumerable<Monkey> monkeys)
    {
        return monkeys
            .OrderByDescending(m => m.InspectionCount)
            .Take(2)
            .Aggregate(1L, (current, monkey) => current * monkey.InspectionCount);
    }
}

static class MonkeyGame
{
    public static void RunRound(ImmutableArray<Monkey> monkeys, bool keepingCalm)
    {
        var multipleOfDivisors = monkeys
            .Select(m => m.TestState.Divisor)
            .Aggregate(1L, (current, div) => current * div);

        Func<long, long> calmAdjustment = keepingCalm
            ? static val => val / 3
            : val => val % multipleOfDivisors;

        foreach (var monkey in monkeys)
        {
            Turn(monkey, monkeys, calmAdjustment);
        }
    }

    private static void Turn(
        Monkey monkey,
        ImmutableArray<Monkey> monkeys,
        Func<long, long> calmAdjustment)
    {
        while (monkey.Items.TryDequeue(out var item))
        {
            monkey.InspectionCount++;
            var newWorryLevel = monkey.OperationState.CalculateNewValue(item);
            newWorryLevel = calmAdjustment(newWorryLevel);

            var nextMonkey = monkey.TestState.Test(newWorryLevel);

            monkeys[nextMonkey].Items.Enqueue(newWorryLevel);
        }
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Monkey> ParseMonkeys(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();
        var monkeys = ImmutableArray.CreateBuilder<Monkey>(paragraphs.Length);

        for (var i = 0; i < paragraphs.Length; i++)
        {
            var paragraph = paragraphs[i].Span;
            monkeys.Add(ParseMonkeys(i, paragraph));
        }

        return monkeys.MoveToImmutable();
    }

    private static readonly PcreRegex testRegex = new(@"Test: divisible by (\d+)");
    private static readonly PcreRegex throwRegex = new(@"throw to monkey (\d+)");

    private static Monkey ParseMonkeys(int number, ReadOnlySpan<ReadOnlyMemory<char>> paragraph)
    {
        var items = ParseItems(paragraph[1].Span);
        var operationState = ParseOperation(paragraph[2].Span);
        var testState = ParseTest(paragraph[3..]);

        return new Monkey(number, items, operationState, testState);

        static Queue<long> ParseItems(ReadOnlySpan<char> span)
        {
            var split = span.IndexOf(':');
            var queue = new Queue<long>();

            foreach (var segment in span[(split + 1)..].Tokenize(','))
            {
                queue.Enqueue(long.Parse(segment));
            }

            return queue;
        }

        static OperationState ParseOperation(ReadOnlySpan<char> span)
        {
            var split = span.IndexOf('=');
            var operationText = span[(split + 1)..];

            var operandIndex = operationText.IndexOfAny('*', '+');

            var left = operationText[..operandIndex].Trim();
            var operand = operationText[operandIndex];
            var right = operationText[(operandIndex + 1)..].Trim();

            return new OperationState(new string(left), operand, new string(right));
        }

        static TestState ParseTest(ReadOnlySpan<ReadOnlyMemory<char>> span)
        {
            var match = testRegex.Match(span[0].Span);
            var divisor = long.Parse(match.Groups[1].Value);
            var trueMonkey = int.Parse(throwRegex.Match(span[1].Span).Groups[1].Value);
            var falseMonkey = int.Parse(throwRegex.Match(span[2].Span).Groups[1].Value);

            return new TestState(divisor, trueMonkey, falseMonkey);
        }
    }
}

record Monkey(
    int Number,
    Queue<long> Items,
    OperationState OperationState,
    TestState TestState)
{
    public long InspectionCount { get; set; }

    public long Test(long value) => TestState.Test(value);
}

readonly record struct TestState(long Divisor, int TrueMonkey, int FalseMonkey)
{
    public int Test(long value) => (value % Divisor) == 0
            ? TrueMonkey
            : FalseMonkey;
}

readonly record struct OperationState(string LeftValue, char Operand, string RightValue)
{
    public long CalculateNewValue(long current)
    {
        var left = ParseValue(LeftValue, current);
        var right = ParseValue(RightValue, current);

        return Operand switch
        {
            '*' => left * right,
            '+' => left + right,
            _ => throw new InvalidOperationException($"Unable to perform '{Operand}' operation"),
        };

        static long ParseValue(string value, long current) => value.Equals("old", StringComparison.OrdinalIgnoreCase)
            ? current
            : long.Parse(value);
    }
}
