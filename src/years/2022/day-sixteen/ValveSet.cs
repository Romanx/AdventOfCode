using CommunityToolkit.HighPerformance.Helpers;

namespace DaySixteen2022;

public readonly record struct ValveSet(ulong Value)
{
    public static Builder CreateBuilder() => new();

    public static ValveSet New() => new();

    public static ValveSet New(IEnumerable<int> indexes)
    {
        var builder = new Builder();
        builder.SetRange(indexes);
        return builder.ToValveSet();
    }

    public ValveSet Set(int index) => this with
    {
        Value = BitHelper.SetFlag(Value, index, true),
    };

    public ValveSet Unset(int index) => this with
    {
        Value = BitHelper.SetFlag(Value, index, false),
    };

    public ValveSet SetRange(IEnumerable<int> indexes)
    {
        var next = Value;
        foreach (var i in indexes)
        {
            BitHelper.SetFlag(ref next, i, true);
        }

        return new ValveSet(next);
    }

    public bool Contains(int index) => BitHelper.HasFlag(Value, index);

    public static ValveSet operator +(ValveSet a, int index) => a.Set(index);

    public static ValveSet operator -(ValveSet a, int index) => a.Unset(index);

    public class Builder
    {
        private ulong _value;

        public void SetRange(IEnumerable<int> indexes)
        {
            foreach (var i in indexes)
            {
                Set(i);
            }
        }

        public void UnsetRange(IEnumerable<int> indexes)
        {
            foreach (var i in indexes)
            {
                Unset(i);
            }
        }

        public void Set(int index) => BitHelper.SetFlag(ref _value, index, true);

        public void Unset(int index) => BitHelper.SetFlag(ref _value, index, false);

        public ValveSet ToValveSet() => new(_value);

        public static Builder operator +(Builder a, int index)
        {
            a.Set(index);
            return a;
        }

        public static Builder operator -(Builder a, int index)
        {
            a.Unset(index);
            return a;
        }
    }
}
