using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using static Shared.AlphabetHelper;

namespace DaySix2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 06), "Custom Customs");

        public override void PartOne(IInput input, IOutput output)
        {
            var groups = input.Parse();

            var counts = groups.Select(g => g.QuestionsAnyoneAnsweredYes);

            output.WriteProperty("Group Count", groups.Length);
            output.WriteProperty("Positive Answers Sum", counts.Sum());
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var groups = input.Parse();

            var counts = groups.Select(g => g.QuestionsEveryoneAnsweredYes);

            output.WriteProperty("Group Count", groups.Length);
            output.WriteProperty("Positive Answers Sum", counts.Sum());
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Group> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Group>();
            uint people = 0;
            var questions = BuildQuestions();

            foreach (var line in input.Lines.AsMemory())
            {
                if (line.IsEmpty)
                {
                    builder.Add(new Group(people, questions.ToImmutableDictionary()));
                    people = 0;
                    questions = BuildQuestions();
                    continue;
                }

                people++;
                foreach (var c in line.Span)
                {
                    questions[c]++;
                }
            }

            if (people > 0)
            {
                builder.Add(new Group(people, questions.ToImmutableDictionary()));
            }

            return builder.ToImmutable();

            static Dictionary<char, int> BuildQuestions()
            {
                return new string(Lowercase)
                    .ToDictionary(k => k, v => 0);
            }
        }
    }

    record Group(uint PersonCount, ImmutableDictionary<char, int> Questions)
    {
        public int QuestionsAnyoneAnsweredYes => Questions.Where(static kvp => kvp.Value > 0).Count();

        public int QuestionsEveryoneAnsweredYes => Questions.Where(kvp => kvp.Value == PersonCount).Count();
    }
}
