namespace DayEighteen2021;

abstract class SnailfishNumber
{
    public SnailfishNumber? Parent { get; set; }

    public abstract int Magnitude();

    public abstract bool Split();

    public abstract RegularNumber[] RegularNumbersInOrder();

    public abstract (PairNumber Pair, int Depth)[] PairsInOrderWithDepth(int depth = 0);

    public bool Explode()
    {
        var pairs = Root.PairsInOrderWithDepth();
        var targetPair = pairs.FirstOrDefault(pair => pair.Depth == 4).Pair;
        if (targetPair is not null)
        {
            var regularNumbers = Root.RegularNumbersInOrder();
            UpdateValue(regularNumbers, targetPair.Left, -1);
            UpdateValue(regularNumbers, targetPair.Right, 1);
            ((PairNumber)targetPair.Parent!).ChildHasExploded(targetPair);
            return true;
        }

        return false;

        static void UpdateValue(RegularNumber[] regularNumbers, SnailfishNumber number, int adjustment)
        {
            var index = Array.FindIndex(regularNumbers, rn => rn == number) + adjustment;
            regularNumbers.ElementAtOrDefault(index)?.AddValue((RegularNumber)number);
        }
    }

    public void Reduce()
    {
        bool actionPerformed;
        do
        {
            actionPerformed = Explode() || Split();
        } while (actionPerformed);
    }

    protected SnailfishNumber Root => Parent is null ? this : Parent;

    public static SnailfishNumber Parse(ReadOnlySpan<char> input)
    {
        var stack = new Stack<SnailfishNumber>();
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsDigit(c))
            {
                stack.Push(new RegularNumber(c - '0'));
            }
            else if (c is ']')
            {
                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(new PairNumber(left, right));
            }
        }

        return stack.Pop();
    }

    public static SnailfishNumber operator +(SnailfishNumber first, SnailfishNumber second)
    {
        var next = new PairNumber(first, second);
        next.Reduce();

        return next;
    }
}
