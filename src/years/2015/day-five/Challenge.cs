using System;
using NodaTime;
using Shared;
using MoreLinq;
using System.Text.RegularExpressions;

namespace DayFive2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 5), "Doesn't He Have Intern-Elves For This?");

        public override void PartOne(IInput input, IOutput output)
        {
            var niceCount = 0;
            foreach (var line in input.Lines.AsMemory())
            {
                if (IsNice(line.Span))
                {
                    niceCount++;
                }
            }

            output.WriteProperty("Nice Count", niceCount);

            static bool IsNice(ReadOnlySpan<char> span)
            {
                var vowelCount = IsVowel(span[0])
                    ? 1
                    : 0;

                var doubles = 0;

                var last = span[0];
                foreach (var current in span[1..])
                {
                    if (IsVowel(current))
                    {
                        vowelCount++;
                    }

                    if (last == current)
                    {
                        doubles++;
                    }
                    else if (IsBad(last, current))
                    {
                        return false;
                    }

                    last = current;
                }

                return vowelCount >= 3 && doubles >= 1;

                static bool IsVowel(char c) => c switch
                {
                    'a' or 'e' or 'i' or 'o' or 'u' => true,
                    _ => false
                };

                static bool IsBad(char first, char last)
                {
                    return (first, last) switch
                    {
                        ('a', 'b') => true,
                        ('c', 'd') => true,
                        ('p', 'q') => true,
                        ('x', 'y') => true,
                        _ => false
                    };
                }
            }
        }

        static readonly Regex HasSplitRepeatLetter = new("([a-z])[a-z]\\1");
        static readonly Regex HasContainsPair = new("([a-z][a-z]).*\\1");

        public override void PartTwo(IInput input, IOutput output)
        {
            var niceCount = 0;
            foreach (var line in input.Lines.AsString())
            {
                if (IsNice(line))
                {
                    niceCount++;
                }
            }

            output.WriteProperty("Nice Count", niceCount);

            static bool IsNice(string line)
            {
                return HasSplitRepeatLetter.IsMatch(line) &&
                       HasContainsPair.IsMatch(line);
            }
        }
    }
}
