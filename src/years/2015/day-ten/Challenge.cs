using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayTen2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 10), "Elves Look, Elves Say");

        public override void PartOne(IInput input, IOutput output)
        {
            var x = input.AsString().Select(c => c - '0').ToArray();
            var result = RunProcess(x)
                .Skip(39)
                .First();

            output.WriteProperty("Result Length", result.Length);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var x = input.AsString().Select(c => c - '0').ToArray();
            var result = RunProcess(x)
                .Skip(49)
                .First();

            output.WriteProperty("Result Length", result.Length);
        }

        public static IEnumerable<int[]> RunProcess(int[] sequence)
        {
            while (true)
            {
                sequence = Iteration(sequence);
                yield return sequence;
            }

            static int[] Iteration(ReadOnlySpan<int> sequence)
            {
                var result = new List<int>();

                var item = sequence[0];
                var count = 1;

                for (var i = 1; i < sequence.Length; i++)
                {
                    if (item == sequence[i])
                    {
                        count++;
                    }
                    else
                    {
                        result.Add(count);
                        result.Add(item);
                        item = sequence[i];
                        count = 1;
                    }
                }

                result.Add(count);
                result.Add(item);

                return result.ToArray();
            }
        }
    }
}
