namespace DaySeventeen2022;

class Pattern
{
    private readonly ReadOnlyMemory<char> pattern;

    public Pattern(ReadOnlyMemory<char> sequence)
    {
        pattern = sequence;
        Index = 0;
    }

    public char Current => pattern.Span[Index];

    public int Index { get; private set; }

    public void Next()
    {
        Index = (Index + 1) % pattern.Length;
    }

    public char GetAndMoveNext()
    {
        var c = Current;
        Next();
        return c;
    }
}
