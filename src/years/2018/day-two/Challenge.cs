using System.Linq;
using MoreLinq.Extensions;

namespace DayTwo2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 2), "Inventory Management System");

    public void PartOne(IInput input, IOutput output)
    {
        var twoCharacters = new HashSet<string>();
        var threeCharacters = new HashSet<string>();

        foreach (var str in input.Lines.AsString())
        {
            var dictionary = new Dictionary<char, int>(26);
            foreach (var @char in str)
            {
                dictionary.AddOrUpdate(@char, 1, static (key, val) => val + 1);
            }

            foreach (var (key, value) in dictionary)
            {
                if (value is 2)
                {
                    twoCharacters.Add(str);
                }
                else if (value is 3)
                {
                    threeCharacters.Add(str);
                }
            }
        }

        var checkSum = twoCharacters.Count * threeCharacters.Count;

        output.WriteProperty("Checksum", checkSum);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var lines = input.Lines
            .AsMemory()
            .Subsets(2)
            .Where(DiffersBySingleCharacter)
            .First();

        var commonCharacters = FindCommonCharacters(lines[0], lines[1]);

        output.WriteProperty("Common Characters", commonCharacters);

        static string FindCommonCharacters(ReadOnlyMemory<char> first, ReadOnlyMemory<char> second)
        {
            return string.Create(first.Length - 1, (first, second), static (span, state) =>
            {
                var (first, second) = state;

                var index = 0;
                for (var i = 0; i < first.Length; i++)
                {
                    if (first.Span[i] == second.Span[i])
                    {
                        span[index] = first.Span[i];
                        index++;
                    }
                }
            });
        }

        static bool DiffersBySingleCharacter(IList<ReadOnlyMemory<char>> arg)
        {
            var first = arg[0].Span;
            var second = arg[1].Span;

            var length = Math.Min(first.Length, second.Length);
            var count = 0;

            for (var index = 0; index < length; index++)
            {
                if (first[index] != second[index])
                {
                    count++;
                    if (count > 1)
                    {
                        return false;
                    }
                }
            }

            return count == 1;
        }
    }
}

internal static class ParseExtensions
{
}
