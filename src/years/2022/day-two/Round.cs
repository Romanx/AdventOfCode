using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DayTwo2022;

readonly record struct Round(Selection Opponent, Selection Player)
    : ISpanParsable<Round>
{
    public int Score { get; } = CalculateRoundScore(Opponent, Player) + (int)Player;

    static int CalculateRoundScore(Selection opponent, Selection player)
    {
        const int Winner = 6;
        const int Loser = 0;
        const int Draw = 3;

        if (opponent == player)
        {
            return Draw;
        }

        return (opponent, player) switch
        {
            (Selection.Rock, Selection.Paper) => Winner,
            (Selection.Rock, Selection.Scissors) => Loser,
            (Selection.Paper, Selection.Rock) => Loser,
            (Selection.Paper, Selection.Scissors) => Winner,
            (Selection.Scissors, Selection.Rock) => Winner,
            (Selection.Scissors, Selection.Paper) => Loser,
            _ => throw new UnreachableException(),
        };
    }

    public static Round Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
    {
        var space = s.IndexOf(' ');

        var opponent = ParseSelection(s[..space]);
        var player = ParseSelection(s[(space + 1)..]);

        return new Round(opponent, player);

        static Selection ParseSelection(ReadOnlySpan<char> s)
        {
            return s switch
            {
                "A" => Selection.Rock,
                "X" => Selection.Rock,
                "B" => Selection.Paper,
                "Y" => Selection.Paper,
                "C" => Selection.Scissors,
                "Z" => Selection.Scissors,
                _ => throw new NotImplementedException($"Not implemented case for {s}"),
            };
        }
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Round result)
    {
        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch 
        {
            result = default;
            return false;
        }
    }

    public static Round Parse(string s, IFormatProvider? provider = null)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Round result)
        => TryParse(s.AsSpan(), provider, out result);
}
