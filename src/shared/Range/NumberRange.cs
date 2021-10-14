namespace Shared
{
    public readonly struct NumberRange
    {
        public NumberRange(uint start, uint end)
        {
            Start = start;
            End = end;
        }

        public uint Start { get; }

        public uint End { get; }

        public bool Contains(uint number) => number >= Start && number <= End;

        public bool Contains(NumberRange other) => other.Start >= Start && other.End <= End;

        public override string ToString() => $"{Start}..{End}";

        internal void Deconstruct(out uint start, out uint end)
        {
            (start, end) = (Start, End);
        }
    }
}
