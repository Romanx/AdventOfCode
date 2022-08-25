namespace DayFour2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 4), "Secure Container");

    public void PartOne(IInput input, IOutput output)
    {
        var (start, end) = input.Parse();

        var possiblePasswords = PossiblePasswords(start, end)
            .Where(p => p.Length == 6)
            .Where(HasAdjacentDigits)
            .Where(EachDigitIncrements)
            .Count();

        output.WriteProperty("Number of possible passwords", possiblePasswords);

        static bool HasAdjacentDigits(string password)
        {
            for (var i = 0; i < password.Length - 1; i++)
            {
                var a = password[i];
                var b = password[i + 1];
                if (a == b)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (start, end) = input.Parse();

        var possiblePasswords = PossiblePasswords(start, end)
            .Where(p => p.Length == 6)
            .Where(HasAdjacentDigitsNotInGroup)
            .Where(EachDigitIncrements)
            .Count();

        output.WriteProperty("Number of possible passwords", possiblePasswords);
    }

    private IEnumerable<string> PossiblePasswords(int start, int end)
    {
        var i = start;
        while (i != end)
        {
            yield return i.ToString();
            i++;
        }
    }

    private static bool EachDigitIncrements(string password)
    {
        for (var i = 0; i < password.Length - 1; i++)
        {
            var a = password[i] - '0';
            var b = password[i + 1] - '0';
            if (b < a)
            {
                return false;
            }
        }

        return true;
    }

    public bool HasAdjacentDigitsNotInGroup(string password)
    {
        var dict = password.Distinct()
                .ToDictionary(k => k, v => 1);

        for (var i = 0; i < password.Length - 1; i++)
        {
            var a = password[i];
            var b = password[i + 1];
            if (a == b)
            {
                dict[a]++;
            }
        }

        return dict.Values.Any(i => i == 2);
    }
}

internal static class ParseExtensions
{
    public static (int Start, int End) Parse(this IInput input)
    {
        var span = input.Content.AsSpan();
        var index = span.IndexOf('-');

        var start = span[0..index];
        var end = span[(index + 1)..];

        return (int.Parse(start), int.Parse(end));
    }
}
