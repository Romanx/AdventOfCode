namespace DayTwentyThree2021;

readonly record struct Amphipod
{
    public static Amphipod Amber { get; } = new('A');
    public static Amphipod Bronze { get; } = new('B');
    public static Amphipod Copper { get; } = new('C');
    public static Amphipod Desert { get; } = new('D');

    public static readonly char[] AllTypes = new char[] { 'A', 'B', 'C', 'D' };

    private Amphipod(char type)
    {
        Type = type;
        MovementMultiplier = type switch
        {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            'D' => 1000,
            _ => throw new InvalidOperationException("Not a valid type!"),
        };
    }

    public char Type { get; }

    public int MovementMultiplier { get; }

    public override string ToString() => $"{Type}";

    public static Amphipod Parse(char type) => type switch
    {
        'A' => Amber,
        'B' => Bronze,
        'C' => Copper,
        'D' => Desert,
        _ => throw new InvalidOperationException("Not a valid type!"),
    };
}
