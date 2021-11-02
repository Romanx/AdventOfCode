using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using static Shared.InstructionHelper;

namespace DaySixteen2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 16), "Chronal Classification");

        public override void PartOne(IInput input, IOutput output)
        {
            var (inputs, _) = input.Parse();

            var count = 0;

            foreach (var testcase in inputs)
            {
                var results = FindInstructionMatches(testcase);
                if (results.Length >= 3)
                {
                    count++;
                }
            }

            output.WriteProperty("Test cases with 3 or more opcodes", count);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var (inputs, program) = input.Parse();
            var allCandidates = FindCandidates(inputs);

            var instructionToCodes = InstructionToPossibleCodes(allCandidates);
            var instructionMap = ReduceCandidates(instructionToCodes);

            foreach (var (instruction, candidate) in instructionMap)
            {
                output.WriteProperty($"Instruction '{instruction}'", candidate.Name);
            }

            var registers = ExecuteProgram(program, instructionMap);

            output.WriteProperty($"Value in register 0", registers[0]);

            static ImmutableArray<int> ExecuteProgram(ImmutableArray<Command> program, ImmutableDictionary<int, Instruction> instructionMap)
            {
                var registers = new int[4];
                registers.AsSpan().Fill(0);
                foreach (var command in program)
                {
                    var instruction = instructionMap[command.Opcode];
                    instruction.Action(registers, command.A, command.B, command.C);
                }

                return registers.ToImmutableArray();
            }

            static ImmutableDictionary<int, Instruction> ReduceCandidates(ImmutableDictionary<Instruction, ImmutableHashSet<int>> inverted)
            {
                var builder = ImmutableDictionary.CreateBuilder<int, Instruction>();
                var scratch = inverted.
                    ToDictionary(k => k.Key, v => v.Value.ToHashSet());

                while (scratch.Count > 0)
                {
                    var items = scratch
                        .Where(kvp => kvp.Value.Count == 1)
                        .Select(kvp => (kvp.Key, kvp.Value.First()))
                        .ToArray();

                    foreach (var (instruction, code) in items)
                    {
                        builder[code] = instruction;
                        scratch.Remove(instruction);
                        foreach (var (_, funcs) in scratch)
                        {
                            funcs.Remove(code);
                        }
                    }
                }

                return builder.ToImmutable();
            }

            static ImmutableDictionary<Instruction, ImmutableHashSet<int>> InstructionToPossibleCodes(ImmutableDictionary<int, ImmutableHashSet<int>> allCandidates)
            {
                return allCandidates
                    .SelectMany(kvp => kvp.Value.Select(i => (Instruction: Instructions[i], Opcode: kvp.Key)))
                    .GroupBy(kvp => kvp.Instruction)
                    .ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Select(i => i.Opcode).ToImmutableHashSet());
            }
        }

        private static ImmutableDictionary<int, ImmutableHashSet<int>> FindCandidates(ImmutableArray<TestCase> inputs)
        {
            var tracking = new Dictionary<int, HashSet<int>>();
            for (var i = 0; i < 16; i++)
            {
                tracking[i] = new HashSet<int>();
            }

            foreach (var testcase in inputs)
            {
                var results = FindInstructionMatches(testcase);
                tracking[testcase.Command.Opcode].UnionWith(results);
            }

            return tracking.ToImmutableDictionary(k => k.Key, v => v.Value.ToImmutableHashSet());
        }

        private static ImmutableArray<int> FindInstructionMatches(TestCase testcase)
        {
            var command = testcase.Command;
            var matches = ImmutableArray.CreateBuilder<int>();
            for (var i = 0; i < Instructions.Length; i++)
            {
                var instruction = Instructions[i];
                var input = testcase.Before.ToArray();
                instruction.Action(input, command.A, command.B, command.C);

                if (input.SequenceEqual(testcase.After))
                {
                    matches.Add(i);
                }
            }

            return matches.ToImmutable();
        }
    }

    record Command(int Opcode, int A, int B, int C);

    record TestCase(ImmutableArray<int> Before, ImmutableArray<int> After, Command Command);
}
