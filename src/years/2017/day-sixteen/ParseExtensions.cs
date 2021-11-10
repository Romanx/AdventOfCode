using Shared.Parser;

namespace DaySixteen2017
{
    internal static class ParseExtensions
    {
        private static readonly CommandParser<Instruction> parser = new CommandParser<Instruction>()
            .AddType<Spin>()
            .AddType<Exchange>()
            .AddType<Partner>();

        public static ImmutableArray<Instruction> ParseInstructions(this IInputContent content)
        {
            var transformed = content
                .Transform(str => str.Split(',', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries))
                .Select(str => str.AsMemory());

            return parser.ParseCommands(transformed)
                .ToImmutableArray();
        }


        [CommandRegex(@"s(\d+)")]
        record Spin(int X) : Instruction("Spin")
        {
            public override void Apply(Span<char> span)
            {
                Span<char> scratch = stackalloc char[span.Length];
                scratch.ConcatInto(span[^X..], span[..^X]);
                scratch.CopyTo(span);
            }

            public static Spin Build(ref PcreRefMatch.GroupList groups)
            {
                return new Spin(int.Parse(groups[1].Value));
            }
        }

        [CommandRegex(@"x(\d+)\/(\d+)")]
        record Exchange(int A, int B) : Instruction("Exchange")
        {
            public override void Apply(Span<char> span)
            {
                var aVal = span[A];
                var bVal = span[B];

                span[A] = bVal;
                span[B] = aVal;
            }

            public static Exchange Build(ref PcreRefMatch.GroupList groups)
            {
                var x = int.Parse(groups[1].Value);
                var y = int.Parse(groups[2].Value);

                return new Exchange(x, y);
            }
        }

        [CommandRegex(@"p([a-z])\/([a-z])")]
        record Partner(char A, char B) : Instruction("Partner")
        {
            public override void Apply(Span<char> span)
            {
                var aIndex = span.IndexOf(A);
                var bIndex = span.IndexOf(B);

                span[aIndex] = B;
                span[bIndex] = A;
            }

            public static Partner Build(ref PcreRefMatch.GroupList groups)
            {
                var x = groups[1].Value[0];
                var y = groups[2].Value[0];

                return new Partner(x, y);
            }
        }
    }

    abstract record Instruction(string Name)
    {
        public abstract void Apply(Span<char> span);
    }
}
