using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime;
using Shared;

namespace DayEight2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 08), "Handheld Halting");

        public override void PartOne(IInput input, IOutput output)
        {
            var instructions = input.ParseInstructions();
            var (accumulator, _) = Computer.RunInstructions(instructions);

            output.WriteProperty("Accumulator before loop", accumulator);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.ParseInstructions();
            var targetQueue = new Queue<int>(Enumerable.Range(0, instructions.Length).Where(i =>
            {
                return instructions[i].Type is "nop" or "jmp";
            }));

            int? finalAccumulator = null;
            while (targetQueue.TryDequeue(out var index))
            {
                var candidate = instructions[index];
                var replacement = candidate switch
                {
                    { Type: "nop" } => candidate with { Type = "jmp" },
                    { Type: "jmp" } => candidate with { Type = "nop" },
                    _ => throw new InvalidOperationException()
                };

                var altered = instructions.Replace(candidate, replacement, ReferenceEqualityComparer.Instance);
                var (accumulator, finished) = Computer.RunInstructions(altered);

                if (finished)
                {
                    finalAccumulator = accumulator;
                    break;
                }
            }

            if (finalAccumulator.HasValue is false)
            {
                throw new InvalidOperationException("Tried everything captain!!");
            }

            output.WriteProperty("Final Accumulator Value", finalAccumulator!.Value);
        }
    }

    public static class ParsingExtensions
    {
        private static readonly Regex instructionRegex = new Regex("(.*) ([+-]+\\d*)");

        public static ImmutableArray<Instruction> ParseInstructions(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Instruction>();
            foreach (var line in input.AsLines())
            {
                var match = instructionRegex.Match(line.ToString());

                builder.Add(new Instruction(match.Groups[1].Value, int.Parse(match.Groups[2].Value)));
            }

            return builder.ToImmutable();
        }
    }
}
