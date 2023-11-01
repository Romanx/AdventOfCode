using MoreLinq;

namespace DayTwentyOne2022;

internal abstract record Monkey(string Name)
{
    public abstract long Yell();

    public abstract ImmutableHashSet<Monkey> FindHumanPath();

    public long CalculateHumanValue()
    {
        var path = FindHumanPath();

        return CalculateHumanValue(
            path,
            0L);
    }

    public abstract long CalculateHumanValue(
        IReadOnlySet<Monkey> humanPath,
        long incomingValue);
}

internal record NumberMonkey(string Name, long Number) : Monkey(Name)
{
    public override string ToString()
        => $"{Name}: {Number}";

    public override long CalculateHumanValue(
        IReadOnlySet<Monkey> humanPath,
        long incomingValue)
        => Name is "humn" ? incomingValue : Number;

    public override long Yell() => Number;

    public override ImmutableHashSet<Monkey> FindHumanPath()
        => Name is "humn"
            ? [this]
            : ImmutableHashSet<Monkey>.Empty;
}

internal record FormulaMonkey(
    string Name,
    Monkey Left,
    Monkey Right,
    char Operator) : Monkey(Name)
{
    public override long CalculateHumanValue(IReadOnlySet<Monkey> humanPath, long incomingValue)
    {
        if (Name is "root")
        {
            return humanPath.Contains(Left)
                ? Left.CalculateHumanValue(humanPath, Right.Yell())
                : Right.CalculateHumanValue(humanPath, Left.Yell());
        }
        else if (humanPath.Contains(Left))
        {
            var value = Operator switch
            {
                '+' => incomingValue - Right.Yell(),
                '-' => incomingValue + Right.Yell(),
                '*' => incomingValue / Right.Yell(),
                '/' => incomingValue * Right.Yell(),
                _ => throw new NotImplementedException(),
            };

            return Left.CalculateHumanValue(humanPath, value);
        }
        else
        {
            var value = Operator switch
            {
                '+' => incomingValue - Left.Yell(),
                '-' => Left.Yell() - incomingValue,
                '*' => incomingValue / Left.Yell(),
                '/' => Left.Yell() / incomingValue,
                _ => throw new NotImplementedException(),
            };
            return Right.CalculateHumanValue(humanPath, value);
        }
    }

    public override ImmutableHashSet<Monkey> FindHumanPath()
    {
        var leftPath = Left.FindHumanPath();
        var rightPath = Right.FindHumanPath();

        if (leftPath.Count > 0)
        {
            return leftPath.Add(this);
        }
        else if (rightPath.Count > 0)
        {
            return rightPath.Add(this);
        }

        return ImmutableHashSet<Monkey>.Empty;
    }

    public override string ToString()
        => $"{Name}: {Left.Name} {Operator} {Right.Name}";

    public override long Yell() => Operator switch
    {
        '+' => Left.Yell() + Right.Yell(),
        '-' => Left.Yell() - Right.Yell(),
        '*' => Left.Yell() * Right.Yell(),
        '/' => Left.Yell() / Right.Yell(),
        _ => throw new NotImplementedException(),
    };
}
