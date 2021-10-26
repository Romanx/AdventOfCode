using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Collections.Extensions;
using NodaTime;
using Shared;
using Spectre.Console;

namespace DayEight2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 8), "I Heard You Like Registers");

        public override void PartOne(IInput input, IOutput output)
        {
            var parser = CpuParser.BuildParser();
            var instructions = parser
                .ParseCommands(input.Lines)
                .ToImmutableArray();
            var program = new Program(instructions);

            var registry = program.Run();

            output.WriteBlock(() =>
            {
                var table = new Table()
                    .AddColumn("Registry")
                    .AddColumn("Value");

                foreach (var (registry, value) in registry.OrderByDescending(r => r.Value))
                {
                    table.AddRow(registry, value.ToString());
                }

                return table;
            });
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var parser = CpuParser.BuildParser();
            var instructions = parser
                .ParseCommands(input.Lines)
                .ToImmutableArray();
            var program = new Program(instructions);

            var max = program.CalculateMaxMemory();

            output.WriteProperty("Max Registry value", max);
        }
    }

    class Program
    {
        private readonly ImmutableArray<Instruction> _instructions;

        public Program(ImmutableArray<Instruction> instructions)
        {
            _instructions = instructions;
        }

        public DictionarySlim<string, int> Run()
        {
            var registry = new DictionarySlim<string, int>();

            foreach (var instruction in _instructions)
            {
                instruction.Apply(registry);
            }

            return registry;
        }

        public int CalculateMaxMemory()
        {
            var registry = new DictionarySlim<string, int>();
            var max = 0;

            foreach (var instruction in _instructions)
            {
                instruction.Apply(registry);

                max = Math.Max(max, registry.OrderByDescending(x => x.Value).DefaultIfEmpty().First().Value);
            }

            return max;
        }
    }
}
