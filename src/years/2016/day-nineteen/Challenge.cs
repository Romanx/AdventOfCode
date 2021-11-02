using System;
using System.Collections.Generic;
using System.Linq;
using NodaTime;
using Shared;

namespace DayNineteen2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 19), "An Elephant Named Joseph");

        public override void PartOne(IInput input, IOutput output)
        {
            var elves = int.Parse(input.Content.AsSpan());
            var elf = PlayGame(elves);

            output.WriteProperty("Elf", elf);

            static Elf PlayGame(int elves)
            {
                var linkedList = new LinkedList<Elf>(Enumerable.Range(1, elves).Select(i => new Elf((uint)i, 1)));

                var current = linkedList.First!;
                while (linkedList.Count > 1)
                {
                    var next = current.NextOrFirst();

                    var currentVal = current.ValueRef;

                    current.ValueRef = new Elf(
                        currentVal.Number,
                        currentVal.Presents + next.ValueRef.Presents);

                    linkedList.Remove(next);

                    current = current.NextOrFirst();
                }

                return current.Value;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var elves = int.Parse(input.Content.AsSpan());
            var elf = PlayGame(elves);

            output.WriteProperty("Elf", elf);

            static Elf PlayGame(int elves)
            {
                var linkedList = new LinkedList<Elf>(Enumerable.Range(1, elves).Select(i => new Elf((uint)i, 1)));

                var current = linkedList.First!;
                var target = linkedList.Find(linkedList.ElementAt((int)Math.Floor((linkedList.Count) / 2f)))!;

                while (linkedList.Count > 1)
                {
                    var currentVal = current.ValueRef;

                    current.ValueRef = new Elf(
                        currentVal.Number,
                        currentVal.Presents + target.ValueRef.Presents);

                    var nextTarget = linkedList.Count % 2 == 1
                        ? target.NextOrFirst().NextOrFirst()
                        : target.NextOrFirst();

                    linkedList.Remove(target);

                    current = current.NextOrFirst();
                    target = nextTarget;
                }

                return current.Value;
            }
        }
    }

    readonly struct Elf
    {
        public Elf(uint number, uint presents)
        {
            Number = number;
            Presents = presents;
        }

        public uint Number { get; }

        public uint Presents { get; }

        public override string ToString() => $"Elf {Number}: {Presents} {(Presents == 1 ? "Present" : "Presents")}";
    }
}
