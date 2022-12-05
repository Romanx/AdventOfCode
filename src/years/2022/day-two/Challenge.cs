namespace DayTwo2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 2), "Rock Paper Scissors");

    public void PartOne(IInput input, IOutput output)
    {
        var rounds = input.Lines
            .As<Round>()
            .ToImmutableArray();

        var totalScore = rounds.Sum(r => r.Score);

        output.WriteProperty("Total Score", totalScore);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var rounds = input.Lines
            .Transform(str => ParseWithStrategy(str.AsSpan()))
            .ToImmutableArray();

        var totalScore = rounds.Sum(r => r.Score);

        output.WriteProperty("Total Score", totalScore);
    }

    private Round ParseWithStrategy(ReadOnlySpan<char> s)
    {
        var space = s.IndexOf(' ');

        var opponent = s[..space] switch
        {
            "A" => Selection.Rock,
            "B" => Selection.Paper,
            "C" => Selection.Scissors,
            _ => throw new NotImplementedException(),
        };

        var player = s[(space + 1)..] switch
        {
            "X" => FindMatchingSelection(opponent, Result.Lose),
            "Y" => FindMatchingSelection(opponent, Result.Draw),
            "Z" => FindMatchingSelection(opponent, Result.Win),
            _ => throw new NotImplementedException(),
        };

        return new Round(opponent, player);

        static Selection FindMatchingSelection(Selection opponent, Result expectedResult)
        {
            if (expectedResult is Result.Draw)
            {
                return opponent;
            }

            return (opponent, expectedResult) switch
            {
                (Selection.Rock, Result.Win) => Selection.Paper,
                (Selection.Rock, Result.Lose) => Selection.Scissors,
                (Selection.Paper, Result.Win) => Selection.Scissors,
                (Selection.Paper, Result.Lose) => Selection.Rock,
                (Selection.Scissors, Result.Win) => Selection.Rock,
                (Selection.Scissors, Result.Lose) => Selection.Paper,
                _ => throw new NotImplementedException(),
            };
        }
    }
}

enum Selection
{
    Rock = 1,
    Paper = 2,
    Scissors = 3,
}

enum Result
{
    Win,
    Lose,
    Draw
}
