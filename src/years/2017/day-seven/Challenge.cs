using Microsoft.Collections.Extensions;

namespace DaySeven2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 7), "Recursive Circus");

        public override void PartOne(IInput input, IOutput output)
        {
            var program = input.Lines.Parse();

            output.WriteProperty("Bottom Program Name", program.Name);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var program = input.Lines.Parse();

            var unbalanced = FindUnbalanced(program)!;

            var dict = unbalanced.Children
                .GroupBy(x => x.StackWeight)
                .ToDictionary(k => k.Key, v => v.ToArray());

            var targetWeight = dict
                .MaxBy(v => v.Value.Length)
                .Key;

            var incorrect = dict
                .Where(kvp => kvp.Key != targetWeight)
                .Single()
                .Value[0];

            var adjustment = incorrect.StackWeight > targetWeight
                    ? (incorrect.StackWeight - targetWeight) * -1
                    : targetWeight - incorrect.StackWeight;

            output.WriteProperty("Target Weight", incorrect.Weight + adjustment);

            static Program? FindUnbalanced(Program program)
            {
                if (program.Balanced is false && program.Children.All(c => c.Balanced))
                {
                    return program;
                }

                foreach (var child in program.Children)
                {
                    var c = FindUnbalanced(child);
                    if (c is not null)
                    {
                        return c;
                    }
                }

                return null;
            }
        }
    }

    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"(?<Name>[a-z]+) \((?<Weight>[0-9]+)\)(?> -> (?<Children>.*))?");

        public static Program Parse(this IInputLines lines)
        {
            var programs = new DictionarySlim<string, Program>();
            var relationships = new DictionarySlim<string, List<string>>();

            foreach (var line in lines.AsMemory())
            {
                var program = ParseLine(line.Span, relationships);
                programs.GetOrAddValueRef(program.Name) = program;
            }

            ApplyRelationships(programs, relationships);

            var root = programs
                .Select(p => p.Value)
                .Single(p => p.Parent is null);

            CalculateStackWeights(root);

            return root;
        }

        private static Program ParseLine(ReadOnlySpan<char> line, DictionarySlim<string, List<string>> relationships)
        {
            var match = regex.Match(line);

            var name = new string(match.Groups["Name"].Value);
            var weight = uint.Parse(match.Groups["Weight"]);

            if (match.TryGetGroup("Children", out var group) && group.Success)
            {
                var split = new string(group.Value)
                    .Split(',', StringSplitOptions.TrimEntries);

                ref var children = ref relationships.GetOrAddValueRef(name);
                children ??= new List<string>(split.Length);
                children.AddRange(split);
            }

            return new Program { Name = name, Weight = weight };
        }

        private static void ApplyRelationships(DictionarySlim<string, Program> programs, DictionarySlim<string, List<string>> relationships)
        {
            foreach (var (parent, children) in relationships)
            {
                ref var parentProgram = ref programs.GetOrAddValueRef(parent);
                var builder = ImmutableArray.CreateBuilder<Program>();
                builder.AddRange(parentProgram.Children);
                foreach (var child in children)
                {
                    ref var cp = ref programs.GetOrAddValueRef(child);
                    builder.Add(cp);
                    cp.Parent = parentProgram;
                }

                parentProgram.Children = builder.ToImmutable();
            }
        }

        private static uint CalculateStackWeights(Program root)
        {
            var total = root.Weight;
            foreach (var child in root.Children)
            {
                total += CalculateStackWeights(child);
            }

            root.StackWeight = total;
            if (root.Children.Length > 0)
            {
                root.Balanced = root.Children
                    .Select(c => c.StackWeight)
                    .Distinct()
                    .Count() == 1;
            }
            else
            {
                root.Balanced = true;
            }

            return total;
        }
    }

    public class Program
    {
        public string Name { get; set; } = string.Empty;

        public uint Weight { get; set; }

        public Program? Parent { get; set; }

        public ImmutableArray<Program> Children { get; set; } = ImmutableArray<Program>.Empty;

        public uint StackWeight { get; set; }

        public bool Balanced { get; set; }

        public override string ToString() => Children.Length > 0
            ? $"{Name} ({Weight}) -> {string.Join(", ", Children.Select(c => c.Name))}"
            : $"{Name} ({Weight})";
    }
}
