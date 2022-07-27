using static MoreLinq.Extensions.IndexExtension;

namespace DayNine2018;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 9), "Marble Mania");

    public override void PartOne(IInput input, IOutput output)
    {
        var game = input.Parse();

        var (winner, score) = RunGame(game);

        output.WriteProperty("Winning player", winner);
        output.WriteProperty("Score", score);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var game = input.Parse();
        game = game with { MarbleCount = game.MarbleCount * 100 };

        var (winner, score) = RunGame(game);

        output.WriteProperty("Winning player", winner);
        output.WriteProperty("Score", score);
    }

    static (int Winner, ulong Score) RunGame(Game game)
    {
        var players = new ulong[game.NumberOfPlayers];
        var ll = new LinkedList<int>();

        var current = ll.AddFirst(0);
        var player = 0;

        for (var i = 0; i < game.MarbleCount; i++)
        {
            var marble = i + 1;
            if (marble % 23 == 0)
            {
                for (var x = 0; x < 7; x++)
                {
                    current = current.PreviousOrLast();
                }
                players[player] += (ulong)(marble + current.Value);
                var temp = current;
                current = current.NextOrFirst();
                ll.Remove(temp);
            }
            else
            {
                current = ll.AddAfter(current.NextOrFirst(), marble);
            }

            player = (player + 1) % game.NumberOfPlayers;
        }

        var (winner, score) = players.Index()
            .MaxBy(p => p.Value);

        return (winner + 1, score);
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"(\d*) players; last marble is worth (\d*) points");

    public static Game Parse(this IInput input)
    {
        var match = regex.Match(input.Content.AsSpan());

        var numberOfPlayers = int.Parse(match.Groups[1]);
        var marbleCount = int.Parse(match.Groups[2]);

        return new Game(numberOfPlayers, marbleCount);
    }
}

readonly record struct Game(int NumberOfPlayers, int MarbleCount);
