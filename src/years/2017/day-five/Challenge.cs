using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayFive2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 5), "A Maze of Twisty Trampolines, All Alike");

        public override void PartOne(IInput input, IOutput output)
        {
            var ints = input.Lines.Ints().ToImmutableArray();
            var computer = new Computer(ints);
            var steps = computer.Run(Adjustment);

            output.WriteProperty("Number of steps", steps);

            static void Adjustment(int pointer, int offset, Dictionary<int, int> adjustment)
            {
                adjustment[pointer] = adjustment[pointer] + 1;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var ints = input.Lines.Ints().ToImmutableArray();
            var computer = new Computer(ints);
            var steps = computer.Run(Adjustment);

            output.WriteProperty("Number of steps", steps);

            static void Adjustment(int pointer, int offset, Dictionary<int, int> adjustment)
            {
                adjustment[pointer] = offset >= 3
                    ? adjustment[pointer] - 1
                    : adjustment[pointer] + 1;
            }
        }
    }

    class Computer
    {
        private readonly ImmutableArray<int> _offsets;
        private int _pointer;
        private readonly Dictionary<int, int> _adjustment;

        public Computer(ImmutableArray<int> offsets)
        {
            _offsets = offsets;
            _pointer = 0;
            _adjustment = new Dictionary<int, int>();

            for (var i = 0; i < offsets.Length; i++)
            {
                _adjustment[i] = 0;
            }
        }

        public int Run(Action<int, int, Dictionary<int, int>> adjustmentFunction)
        {
            var steps = 0;
            while (_pointer >= 0 && _pointer < _offsets.Length)
            {
                var offset = _offsets[_pointer]
                    + _adjustment[_pointer];

                adjustmentFunction(_pointer, offset, _adjustment);
                _pointer += offset;
                steps++;
            }

            return steps;
        }
    }
}
