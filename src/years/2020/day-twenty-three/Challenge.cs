using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using NodaTime;
using Shared;

namespace DayTwentyThree2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 23), "Crab Cups");

        public override void PartOne(IInput input, IOutput output)
        {
            const int Moves = 100;
            var cups = new LinkedList<int>(input.Content.CharactersToInt());

            var final = PerformMoves(cups, Moves);

            output.WriteProperty("Final Order", string.Join(", ", final));

            var builder = new StringBuilder();
            var currentNode = final.Find(1)!;
            for (var i = 0; i < 8; i++)
            {
                var nextNode = currentNode.Next ?? final.First!;
                builder.Append(nextNode.Value);
                currentNode = nextNode;
            }
            output.WriteProperty("Formatted", builder.ToString());
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const int Moves = 10_000_000;
            var cups = new LinkedList<int>(input.Content.CharactersToInt().Concat(MoreEnumerable.Sequence(10, 1_000_000)));

            var final = PerformMoves(cups, Moves);
            var cupOne = final.Find(1)!;

            var first = cupOne.Next ?? final.First!;
            var second = first.Next ?? final.First!;

            output.WriteProperty("First Cup after 1", first.Value);
            output.WriteProperty("Second Cup after 1", second.Value);
            output.WriteProperty("Result", (long)first.Value * second.Value);
        }

        private static LinkedList<int> PerformMoves(LinkedList<int> cups, int iterations)
        {
            var cupsByValue = new Dictionary<int, LinkedListNode<int>>();
            var currentNode = cups.First;
            var cupCount = cups.Count;

            while (currentNode != null)
            {
                cupsByValue.Add(currentNode.Value, currentNode);
                currentNode = currentNode.Next;
            }

            currentNode = cups.First!;

            for (var move = 1; move <= iterations; move++)
            {
                var currentCup = currentNode.Value;
                currentNode = currentNode.Next ?? cups.First!;

                var taken = new List<LinkedListNode<int>>(3);
                for (var i = 0; i < 3; i++)
                {
                    var nextNode = currentNode.Next ?? cups.First!;
                    cups.Remove(currentNode);
                    taken.Add(currentNode);
                    currentNode = nextNode;
                }

                for (var destination = currentCup - 1; ; destination--)
                {
                    if (destination == 0)
                    {
                        destination = cupCount;
                    }

                    if (!taken.Any(n => n.Value == destination))
                    {
                        var targetNode = cupsByValue[destination];
                        foreach (var placedNode in taken.Reverse<LinkedListNode<int>>())
                        {
                            cups.AddAfter(targetNode, placedNode);
                        }

                        break;
                    }
                }
            }

            return cups;
        }
    }
}
