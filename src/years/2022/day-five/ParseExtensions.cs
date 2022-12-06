namespace DayFive2022;

internal static class ParseExtensions
{
    private static readonly PcreRegex procedureRegex = new(@"move (\d+) from (\d+) to (\d+)");

    public static (Stack<char>[] Stacks, ImmutableArray<Procedure> Procedures) Parse(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();

        var stacks = ParseStacks(paragraphs[0]);
        var procedures = ParseProcedures(paragraphs[1]);

        return (stacks, procedures);
    }

    private static Stack<char>[] ParseStacks(ReadOnlyMemory<ReadOnlyMemory<char>> paragraph)
    {
        var span = paragraph.Span;

        var stackCount = (int)double.Ceiling(span[0].Length / 4.0);

        var stacks = new Stack<char>[stackCount];
        for (var i = 0; i < stackCount; i++)
        {
            stacks[i] = new Stack<char>();
        }

        for (var idx = span.Length - 2; idx >= 0; idx--)
        {
            var start = 1;
            var line = span[idx].Span;
            for (var i = 0; i < stackCount; i++)
            {
                if (line[start] is not ' ')
                {
                    stacks[i].Push(line[start]);
                }

                start += 4;
            }
        }

        return stacks;
    }

    private static ImmutableArray<Procedure> ParseProcedures(ReadOnlyMemory<ReadOnlyMemory<char>> readOnlyMemory)
    {
        var procedures = ImmutableArray.CreateBuilder<Procedure>(readOnlyMemory.Length);

        for (var i = 0; i < readOnlyMemory.Length; i++)
        {
            var span = readOnlyMemory.Span[i].Span;
            var match = procedureRegex.Match(span);
            var count = uint.Parse(match.Groups[1]);
            var source = uint.Parse(match.Groups[2]);
            var target = uint.Parse(match.Groups[3]);

            procedures.Add(new Procedure(count, source, target));
        }

        return procedures.MoveToImmutable();
    }
}
