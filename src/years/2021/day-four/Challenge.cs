using Microsoft.Toolkit.HighPerformance;
using static MoreLinq.Extensions.IndexExtension;

namespace DayFour2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 4), "Giant Squid");

    public override void PartOne(IInput input, IOutput output)
    {
        var grids = input.Parse();
        var winner = grids.MinBy(grid => grid.WinningTurn);

        output.WriteProperty("Winning Turn", winner.WinningTurn);
        output.WriteProperty("Winner Score", winner.FinalScore);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var grids = input.Parse();
        var worstGrid = grids.MaxBy(grid => grid.WinningTurn);

        output.WriteProperty("Last Winning Turn", worstGrid.WinningTurn);
        output.WriteProperty("Last Winner Score", worstGrid.FinalScore);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<BingoGrid> Parse(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();

        var numbersAndTurn = new string(paragraphs[0].Span[0].Span)
            .Split(',')
            .Select(int.Parse)
            .Index(startIndex: 1)
            .ToDictionary(x => x.Value, x => x.Key);

        var grids = ImmutableArray.CreateBuilder<BingoGrid>();
        var scratch = new (int, int)[5, 5];

        foreach (var grid in paragraphs[1..])
        {
            grids.Add(ParseGrid(grid, scratch, numbersAndTurn));
        }

        return grids.ToImmutable();
    }

    static BingoGrid ParseGrid(
        ReadOnlyMemory<ReadOnlyMemory<char>> grid,
        (int Turn, int Number)[,] scratch,
        Dictionary<int, int> numbersOnTurn)
    {
        var winningTurns = new List<int>(10);
        var span = scratch.AsSpan2D();

        for (var y = 0; y < grid.Length; y++)
        {
            var row = new string(grid.Span[y].Span).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var x = 0; x < row.Length; x++)
            {
                var number = int.Parse(row[x]);
                span[x, y] = (numbersOnTurn[number], number);
            }
        }

        for (var y = 0; y < span.Height; y++)
        {
            var last = -1;
            foreach (var (turn, _) in span.GetRow(y))
            {
                last = Math.Max(turn, last);
            }
            winningTurns.Add(last);
        }

        for (var x = 0; x < span.Width; x++)
        {
            var last = -1;
            foreach (var (turn, _) in span.GetColumn(x))
            {
                last = Math.Max(turn, last);
            }
            winningTurns.Add(last);
        }

        var earliestTurn = winningTurns.Min();
        var winningNumber = numbersOnTurn
            .First(x => x.Value == earliestTurn)
            .Key;

        var score = 0;

        for (var y = 0; y < span.Height; y++)
        {
            foreach (var (turn, number) in span.GetRow(y))
            {
                if (turn > earliestTurn)
                {
                    score += number;
                }
            }
        }

        return new BingoGrid(earliestTurn, winningNumber * score);
    }
}

readonly record struct BingoGrid(int WinningTurn, int FinalScore);
