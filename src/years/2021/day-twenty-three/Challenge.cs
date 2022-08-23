using Microsoft.Toolkit.HighPerformance;

namespace DayTwentyThree2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 23), "Amphipod");

    public void PartOne(IInput input, IOutput output)
    {
        var arr = input.Lines.As2DArray();
        var board = Board.Parse(arr);

        var (result, cost) = OrganizeAmphipods(board);

        output.WriteProperty("Result", result);
        output.WriteProperty("Cost", cost);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var arr = input.Lines.As2DArray();
        var board = Board.Parse(ExpandProblem(arr));

        var (result, cost) = OrganizeAmphipods(board);

        output.WriteProperty("Result", result);
        output.WriteProperty("Cost", cost);

        static char[,] ExpandProblem(char[,] arr)
        {
            Span2D<char> insert = new char[,]
            {
                { ' ', ' ', '#', 'D', '#', 'C', '#', 'B', '#', 'A', '#', ' ', ' ' },
                { ' ', ' ', '#', 'D', '#', 'B', '#', 'A', '#', 'C', '#', ' ', ' ' },
            };

            var span = arr.AsSpan2D();
            var expanded = new char[span.Height + 2, span.Width];
            var expandedSpan = expanded.AsSpan2D();

            span[0..3, ..].CopyTo(expandedSpan[0..3, ..]);

            insert.CopyTo(expandedSpan[3..5, ..]);

            span[3.., ..].CopyTo(expandedSpan[5.., ..]);

            return expanded;
        }
    }

    static (Board Board, int Cost) OrganizeAmphipods(Board initialState)
    {
        var queue = new PriorityQueue<(Board Board, int Cost), int>();
        queue.Enqueue((initialState, 0), 0);
        var visited = new HashSet<(Board, int)>();
        var currentCosts = new Dictionary<Board, int>();

        while (queue.TryDequeue(out var current, out _))
        {
            visited.Add(current);

            foreach (var next in current.Board.NextStates())
            {
                if (visited.Contains(next) is false)
                {
                    var newCost = current.Cost + next.Cost;
                    if (newCost < currentCosts.GetValueOrDefault(next.Board, int.MaxValue))
                    {
                        currentCosts[next.Board] = newCost;
                        queue.Enqueue((next.Board, newCost), newCost);
                    }
                }
            }
        }

        var board = currentCosts.Keys
            .First(board => board.Organized);

        return (board, currentCosts[board]);
    }
}

internal static class ParseExtensions
{
}
