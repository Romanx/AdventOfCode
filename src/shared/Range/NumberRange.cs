using System.Numerics;

namespace Shared
{
    public readonly record struct NumberRange<T>(T Start, T End)
        where T : IBinaryNumber<T>
    {
        public bool Contains(T number) => number >= Start && number <= End;

        public bool Contains(NumberRange<T> other) => other.Start >= Start && other.End <= End;

        public override string ToString() => $"{Start}..{End}";
    }
}
