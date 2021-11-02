using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayNine2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 9), "Stream Processing");

        public override void PartOne(IInput input, IOutput output)
        {
            var content = input.Content.AsMemory();

            var group = ParseGroup(content);
            var score = CalculateScore(group);

            output.WriteProperty("Total Score", score);

            static int CalculateScore(Group root, int depth = 1)
            {
                var count = 1 * depth;

                foreach (var group in root.Groups)
                {
                    count += CalculateScore(group, depth + 1);
                }

                return count;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var content = input.Content.AsMemory();

            var group = ParseGroup(content);
            var score = CalculateScore(group);

            output.WriteProperty("Total Garbage Character Count", score);

            static long CalculateScore(Group root)
            {
                var count = root.Garbage.Sum(g => g.ValidCharacterCount);

                foreach (var group in root.Content.OfType<Group>())
                {
                    count += CalculateScore(group);
                }

                return count;
            }
        }

        static Group ParseGroup(ReadOnlyMemory<char> content)
        {
            var (group, _) = ParseGroup(content.Span);
            return group;

            static (Group Group, int Offset) ParseGroup(ReadOnlySpan<char> span)
            {
                if (span[0] != '{')
                {
                    throw new InvalidOperationException("Garbage starts with a '{'");
                }

                var content = ImmutableArray.CreateBuilder<Content>();

                for (var i = 1; i < span.Length; i++)
                {
                    if (span[i] == '{')
                    {
                        var (group, offset) = ParseGroup(span[i..]);
                        content.Add(group);
                        i += offset;
                    }
                    else if (span[i] == '<')
                    {
                        var (garbage, offset) = ParseGarbage(span[i..]);
                        content.Add(garbage);
                        i += offset;
                    }
                    else if (span[i] == '}')
                    {
                        return (new Group(content.ToImmutable()), i);
                    }
                }

                throw new InvalidOperationException("Boom?");
            }

            static (Garbage Garbage, int Offset) ParseGarbage(ReadOnlySpan<char> span)
            {
                if (span[0] != '<')
                {
                    throw new InvalidOperationException("Garbage starts with a '<'");
                }

                var validCount = 0u;
                for (var i = 1; i < span.Length; i++)
                {
                    if (span[i] == '!')
                    {
                        i += 1;
                    }
                    else if (span[i] == '>')
                    {
                        return (new Garbage(validCount), i);
                    }
                    else
                    {
                        validCount++;
                    }
                }

                throw new InvalidOperationException("Boom?");
            }
        }
    }

    record Content();

    record Group(ImmutableArray<Content> Content) : Content()
    {
        public IEnumerable<Group> Groups => Content.OfType<Group>();

        public IEnumerable<Garbage> Garbage => Content.OfType<Garbage>();
    }

    record Garbage(uint ValidCharacterCount) : Content();
}
