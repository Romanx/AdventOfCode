using Microsoft.Toolkit.HighPerformance;

namespace DayTwentyThree2021;

readonly record struct Room
{
    public Room(Amphipod amphipod, int index, string content)
    {
        Amphipod = amphipod;
        Index = index;
        Content = content;
        Organized = content.ToCharArray().All(c => c == amphipod.Type);
        HasSpace = content.AsSpan().IndexOf('.') != -1;
        IsEmptyOrHasAllValidAmphipods = content.ToCharArray().All(c => c == '.' || c == amphipod.Type);
        HasAmpipodsOfIncorrectType = !IsEmptyOrHasAllValidAmphipods;
    }

    public bool Organized { get; }
    public bool HasSpace { get; }
    public bool HasAmpipodsOfIncorrectType { get; }
    public bool IsEmptyOrHasAllValidAmphipods { get; }
    public Amphipod Amphipod { get; }
    public int Index { get; }
    public string Content { get; }

    public (Amphipod Amphipod, int index) RetrieveFirst()
    {
        var index = Content.IndexOfAny(Amphipod.AllTypes);
        return (Amphipod.Parse(Content[index]), index);
    }

    public Room Add()
    {
        if (HasSpace is false)
        {
            throw new InvalidOperationException("Cannot add amphipod, we're full!");
        }

        var target = Content
            .AsSpan()
            .LastIndexOf('.');

        return new Room(Amphipod, Index, Content.ReplaceCharAt(target, Amphipod.Type));
    }

    public override string ToString() => Content;
}
