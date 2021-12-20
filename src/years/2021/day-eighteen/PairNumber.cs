namespace DayEighteen2021;

class PairNumber : SnailfishNumber
{
    public PairNumber(SnailfishNumber left, SnailfishNumber right)
    {
        Left = left;
        Right = right;
        left.Parent = this;
        right.Parent = this;
    }

    public SnailfishNumber Left { get; private set; }

    public SnailfishNumber Right { get; private set; }

    public override int Magnitude() => (Left.Magnitude() * 3) + (Right.Magnitude() * 2);

    public override (PairNumber Pair, int Depth)[] PairsInOrderWithDepth(int depth = 0)
    {
        return Left.PairsInOrderWithDepth(depth + 1)
            .Append((this, depth))
            .Concat(Right.PairsInOrderWithDepth(depth + 1))
            .ToArray();
    }

    public override RegularNumber[] RegularNumbersInOrder()
        => Left.RegularNumbersInOrder().Concat(Right.RegularNumbersInOrder()).ToArray();

    internal void ChildHasExploded(PairNumber child)
    {
        var rn = new RegularNumber(0)
        {
            Parent = this
        };

        if (Left == child)
        {
            Left = rn;
        }
        else
        {
            Right = rn;
        }
    }

    public override bool Split()
    {
        if (Left is RegularNumber ln)
        {
            if (ln.Value >= 10)
            {
                Left = ln.SplitToPair(this);
                return true;
            }
        }

        var didSplit = Left.Split();
        if (didSplit)
        {
            return true;
        }

        if (Right is RegularNumber rn)
        {
            if (rn.Value >= 10)
            {
                Right = rn.SplitToPair(this);
                return true;
            }
        }

        return Right.Split();
    }

    public override string ToString() => $"[{Left}, {Right}]";
}
