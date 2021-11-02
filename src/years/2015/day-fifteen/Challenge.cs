using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using NodaTime;
using Shared;

namespace DayFifteen2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 15), "Science for Hungry People");

        public override void PartOne(IInput input, IOutput output)
        {
            var ingredients = input.Parse();

            var cookie = Distribute(new int[ingredients.Length], 100, 0)
                .Select(subset => new Cookie(ingredients, subset.ToImmutableArray()))
                .MaxBy(profile => profile.Score)
                .First();

            output.WriteProperty($"Cookie", cookie);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var ingredients = input.Parse();

            var cookie = Distribute(new int[ingredients.Length], 100, 0)
                .Select(subset => new Cookie(ingredients, subset.ToImmutableArray()))
                .Where(cookie => cookie.Calories == 500)
                .MaxBy(profile => profile.Score)
                .First();

            output.WriteProperty($"Cookie", cookie);
        }

        IEnumerable<int[]> Distribute(int[] start, int target, int index)
        {
            var remaining = target - start.Sum();
            if (index == start.Length - 1)
            {
                var x = start.ToArray();
                x[index] = remaining;
                yield return x;
            }
            else
            {
                for (var n = 0; n < remaining; n++)
                {
                    var x = start.ToArray();
                    x[index] = n;
                    foreach (var d in Distribute(x, target, index + 1))
                    {
                        yield return d;
                    }
                }
            }
        }
    }
}
