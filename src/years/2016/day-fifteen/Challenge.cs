using System;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq.Extensions;
using NodaTime;
using PCRE;
using Shared;

namespace DayFifteen2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 15), "Timing is Everything");

        public override void PartOne(IInput input, IOutput output)
        {
            var discs = input.Parse();
            var perfectMachine = RunMachineUntilPerfect(new Machine(0, discs));

            output.WriteProperty("Working time", perfectMachine.Time);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var discs = input.Parse();
            var lastDisc = discs.MaxBy(d => d.Number).First();
            discs = discs.Add(new Disc(lastDisc.Number + 1, 11, 0, 0));

            var perfectMachine = RunMachineUntilPerfect(new Machine(0, discs));

            output.WriteProperty("Working time", perfectMachine.Time);
        }

        private static Machine RunMachineUntilPerfect(Machine machine)
        {
            while (true)
            {
                machine = machine.Step();
                if (machine.IsWorking())
                {
                    return machine;
                }
            }
        }
    }

    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new("Disc #(?<DiscNumber>[0-9]+) has (?<PositionCount>[0-9]+) positions; at time=0, it is at position (?<StartPosition>[0-9]+).");

        public static ImmutableArray<Disc> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Disc>();

            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(ParseDisc(line.Span));
            }

            return builder.ToImmutable();
        }

        static Disc ParseDisc(ReadOnlySpan<char> span)
        {
            var match = regex.Match(span);

            return new Disc(
                int.Parse(match.Groups["DiscNumber"].Value),
                int.Parse(match.Groups["PositionCount"].Value),
                int.Parse(match.Groups["StartPosition"].Value),
                int.Parse(match.Groups["StartPosition"].Value));
        }
    }

    record Machine(int Time, ImmutableArray<Disc> Discs)
    {
        public Machine Step()
        {
            var nextTime = Time + 1;
            var builder = ImmutableArray.CreateBuilder<Disc>(Discs.Length);
            foreach (var disc in Discs)
            {
                builder.Add(disc.Step(nextTime));
            }

            return this with
            {
                Time = nextTime,
                Discs = builder.ToImmutable()
            };
        }

        public bool IsWorking()
        {
            return Discs.All(disc => disc.CurrentPosition == 0);
        }
    }

    record Disc(int Number, int NumberOfPositions, int StartPosition, int CurrentPosition)
    {
        public Disc Step(int time)
        {
            return this with
            {
                CurrentPosition = (StartPosition + time + Number) % NumberOfPositions
            };
        }
    }
}
