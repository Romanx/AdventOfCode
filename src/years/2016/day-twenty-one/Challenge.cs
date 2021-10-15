using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using NodaTime;
using Shared;

namespace DayTwentyOne2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 21), "Scrambled Letters and Hash");

        public override void PartOne(IInput input, IOutput output)
        {
            var scrambler = input.Parse();

            var password = "abcdefgh";
            output.WriteProperty("Input", password);
            output.WriteProperty("Scrambled", scrambler.ScramblePassword(password));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var scrambler = input.Parse();

            var scrambled = "fbgdceah";
            output.WriteProperty("Input", scrambled);
            output.WriteProperty("Descrambled", scrambler.DescramblePassword(scrambled));
        }
    }

    record Scrambler(ImmutableArray<ScrambleFunction> Functions)
    {
        public string ScramblePassword(string input, TextWriter? writer = null)
        {
            var state = (Functions, Input: input, Writer: writer);

            return string.Create(input.Length, state, static (span, state) =>
            {
                state.Input.AsSpan().CopyTo(span);
                state.Writer?.WriteLine($"Start: {state.Input}");

                var step = 1;
                foreach (var function in state.Functions)
                {
                    function.ApplyScramble(span);
                    state.Writer?.WriteLine($"Scramble {function.Type} {step}: {new string(span)}");
                    step++;
                }
            });
        }

        public string DescramblePassword(string input, TextWriter? writer = null)
        {
            var state = (Functions, Input: input, Writer: writer);

            return string.Create(input.Length, state, static (span, state) =>
            {
                state.Input.AsSpan().CopyTo(span);
                state.Writer?.WriteLine($"Start: {state.Input}");

                var step = 1;
                foreach (var function in state.Functions.Reverse())
                {
                    function.Descramble(span);
                    state.Writer?.WriteLine($"Scramble {function.Type} {step}: {new string(span)}");
                    step++;
                }
            });
        }
    }
}
