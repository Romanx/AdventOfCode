namespace DayTwentyOne2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 21), "Dirac Dice");

    public override void PartOne(IInput input, IOutput output)
    {
        var players = input.Lines.ParsePlayers();
        var die = new Dice(1, 100);

        var state = new Game(players[0], players[1], true);

        while (state.HasWinner(targetScore: 1000) is false)
        {
            state = state.Next(die.RollThree());
        }

        var losingPlayer = state.Loser;

        output.WriteProperty("Losing Player Score", losingPlayer.Score);
        output.WriteProperty("Dice Rolls", die.NumberOfRolls);
        output.WriteProperty("Multiplied for Result", losingPlayer.Score * die.NumberOfRolls);
    }

    // Number Rolled -> Frequency of Result
    private static readonly ImmutableDictionary<int, long> QuantumDieFrequencies = new Dictionary<int, long>
    {
        [3] = 1,
        [4] = 3,
        [5] = 6,
        [6] = 7,
        [7] = 6,
        [8] = 3,
        [9] = 1
    }.ToImmutableDictionary();

    public override void PartTwo(IInput input, IOutput output)
    {
        var players = input.Lines.ParsePlayers();
        var state = new Game(players[0], players[1], true);
        var cache = new Dictionary<Game, WinCounts>();

        var result = PlayWithQuantumDie(state, cache);

        output.WriteProperty("Player 1 Wins", result.Player1);
        output.WriteProperty("Player 2 Wins", result.Player2);
        output.WriteProperty("Winner total", result.Winner);

        static WinCounts PlayWithQuantumDie(Game state, Dictionary<Game, WinCounts> cache)
        {
            if (state.HasWinner(targetScore: 21))
            {
                return state.Player1.Score > state.Player2.Score
                    ? new(1, 0)
                    : new(0, 1);
            }
            else if (cache.TryGetValue(state, out var cached))
            {
                return cached;
            }
            else
            {
                var totalResult = QuantumDieFrequencies.Select(kvp =>
                {
                    var (die, frequency) = kvp;

                    var result = PlayWithQuantumDie(state.Next(die), cache);
                    return result * frequency;
                }).Aggregate((a, b) => a + b);

                cache.Add(state, totalResult);
                return totalResult;
            }
        }
    }
}

class Dice
{
    private readonly int _max;
    private int _current;

    public int Current
    {
        get => _current;
        private set => _current = ((value - 1) % _max) + 1;
    }

    public int NumberOfRolls { get; private set; }

    public Dice(int start, int max)
    {
        _max = max;
        Current = start;
    }

    public int RollThree()
    {
        var result = 0;
        result += Current++;
        result += Current++;
        result += Current++;
        NumberOfRolls += 3;

        return result;
    }

    public override string ToString() => $"Dice {Current} - Number of Rolls {NumberOfRolls}";
}

readonly record struct WinCounts(long Player1, long Player2)
{
    public static WinCounts operator +(WinCounts a, WinCounts b)
        => new(a.Player1 + b.Player1, a.Player2 + b.Player2);

    public static WinCounts operator *(WinCounts a, long multiplier)
        => new(a.Player1 * multiplier, a.Player2 * multiplier);

    public long Winner => Player1 > Player2 ? Player1 : Player2;
}

readonly record struct Game(Player Player1, Player Player2, bool IsPlayer1Turn)
{
    public Game Next(int die)
    {
        return this with
        {
            Player1 = IsPlayer1Turn ? Player1.Next(die) : Player1,
            Player2 = !IsPlayer1Turn ? Player2.Next(die) : Player2,
            IsPlayer1Turn = !IsPlayer1Turn
        };
    }

    public bool HasWinner(int targetScore) =>
        Player1.Score >= targetScore || Player2.Score >= targetScore;

    public Player Loser => new[] { Player1, Player2 }.MinBy(x => x.Score);
}

readonly record struct Player(int PlayerNumber, int CurrentPosition, int Score)
{
    public Player Next(int die)
    {
        var nextPosition = (CurrentPosition + die - 1) % 10 + 1;

        return this with
        {
            CurrentPosition = nextPosition,
            Score = Score + nextPosition
        };
    }

    public override string ToString() => $"Player {PlayerNumber} at position: {CurrentPosition} with score {Score}";
}

internal static class ParseExtensions
{
    public static ImmutableArray<Player> ParsePlayers(this IInputLines lines)
    {
        var builder = ImmutableArray.CreateBuilder<Player>();
        var regex = new PcreRegex(@"Player (\d) starting position: (\d)");
        foreach (var line in lines.AsMemory())
        {
            var match = regex.Match(line.Span);

            builder.Add(new Player(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                0));
        }

        return builder.ToImmutable();
    }
}
