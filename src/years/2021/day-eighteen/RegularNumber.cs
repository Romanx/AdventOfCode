namespace DayEighteen2021;

class RegularNumber : SnailfishNumber
{
    public int Value { get; private set; }

    public RegularNumber(int value)
    {
        Value = value;
    }

    public override int Magnitude() => Value;

    public override (PairNumber Pair, int Depth)[] PairsInOrderWithDepth(int depth = 0)
        => Array.Empty<(PairNumber Pair, int Depth)>();

    public override RegularNumber[] RegularNumbersInOrder()
        => new[] { this };

    public override bool Split() => false;

    public override string ToString() => Value.ToString();

    public void AddValue(RegularNumber number)
    {
        Value += number.Value;
    }

    internal PairNumber SplitToPair(SnailfishNumber parent)
    {
        var left = new RegularNumber((int)Math.Floor(Value / 2f));
        var right = new RegularNumber((int)Math.Ceiling(Value / 2f));

        return new PairNumber(left, right)
        {
            Parent = parent
        };
    }
}
