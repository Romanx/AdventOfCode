using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Shared;

namespace DayOne2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 1), "Inverse Captcha");

        public override void PartOne(IInput input, IOutput output)
        {
            var list = input.Content.AsLinkedList();

            output.WriteProperty("Produce Sum", Sum(list));

            static int Sum(LinkedList<int> list)
            {
                var items = new List<int>();
                var start = list.First;
                while (start is not null)
                {
                    if (start.ValueRef == start.NextOrFirst().ValueRef)
                    {
                        items.Add(start.ValueRef);
                    }

                    start = start.Next;
                }

                return items.Sum();
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var list = input.Content.AsLinkedList();

            output.WriteProperty("Produce Sum", Sum(list));

            static int Sum(LinkedList<int> list)
            {
                var items = new List<int>();
                var start = list.First;
                var halfway = list.At(list.Count / 2);

                while (start is not null)
                {
                    if (start.ValueRef == halfway.ValueRef)
                    {
                        items.Add(start.ValueRef);
                    }

                    start = start.Next;
                    halfway = halfway.NextOrFirst();
                }

                return items.Sum();
            }
        }
    }

    internal static class ParseExtensions
    {
        public static LinkedList<int> AsLinkedList(this IInputContent content)
            => new(content.CharactersToInt());
    }
}
