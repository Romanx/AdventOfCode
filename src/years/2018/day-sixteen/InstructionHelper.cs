﻿using System.Collections.Immutable;

namespace DaySixteen2018
{
    public static class InstructionHelper
    {
        public delegate void InstructionAction(int[] registers, int a, int b, int output);

        public static ImmutableArray<Instruction> Instructions { get; } = new[]
        {
            // Addition
            new Instruction("addr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] + registers[b];
            }),
            new Instruction("addi", (registers, a, b, output) =>
            {
                registers[output] = registers[a] + b;
            }),

            // Multiplication
            new Instruction("mulr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] * registers[b];
            }),
            new Instruction("muli", (registers, a, b, output) =>
            {
                registers[output] = registers[a] * b;
            }),

            // Bitwise AND
            new Instruction("banr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] & registers[b];
            }),
            new Instruction("bani", (registers, a, b, output) =>
            {
                registers[output] = registers[a] & b;
            }),

            // Bitwise OR
            new Instruction("borr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] | registers[b];
            }),
            new Instruction("bori", (registers, a, b, output) =>
            {
                registers[output] = registers[a] | b;
            }),

            // Assinment
            new Instruction("setr", (registers, a, _, output) =>
            {
                registers[output] = registers[a];
            }),
            new Instruction("seti", (registers, a, _, output) =>
            {
                registers[output] = a;
            }),

            // Greater-than testing
            new Instruction("gtir", (registers, a, b, output) =>
            {
                registers[output] = a > registers[b]
                    ? 1
                    : 0;
            }),
            new Instruction("gtri", (registers, a, b, output) =>
            {
                registers[output] = registers[a] > b
                    ? 1
                    : 0;
            }),
            new Instruction("gtrr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] > registers[b]
                    ? 1
                    : 0;
            }),

            // Equality testing
            new Instruction("gtir", (registers, a, b, output) =>
            {
                registers[output] = a == registers[b]
                    ? 1
                    : 0;
            }),
            new Instruction("gtri", (registers, a, b, output) =>
            {
                registers[output] = registers[a] == b
                    ? 1
                    : 0;
            }),
            new Instruction("gtrr", (registers, a, b, output) =>
            {
                registers[output] = registers[a] == registers[b]
                    ? 1
                    : 0;
            }),
        }.ToImmutableArray();

        public record Instruction(string Name, InstructionAction Action);
    }
}
