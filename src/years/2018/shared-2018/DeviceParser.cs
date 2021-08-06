﻿using System;
using System.Collections.Immutable;
using System.Linq;

namespace Shared
{
    public static class DeviceParser
    {
        public static Device ParseDevice(this IInput input)
        {
            var (instructionPointer, commands) = input.Parse();

            return new Device(instructionPointer, commands);
        }

        public static (int InstructionPointer, ImmutableArray<Command> Commands) Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Command>();
            var lines = input.AsLines().ToArray().AsSpan();
            var instructionPointer = int.Parse(lines[0].Span[3..]);

            foreach (var line in lines[1..])
            {
                builder.Add(ToCommand(line));
            }

            return (instructionPointer, builder.ToImmutable());

            static Command ToCommand(ReadOnlyMemory<char> line)
            {
                var items = line.ToString().Split(' ');

                return new Command(items[0], int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]));
            }
        }
    }
}