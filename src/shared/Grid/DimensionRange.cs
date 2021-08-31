namespace Shared.Grid
{
    public record DimensionRange(int Min, int Max)
    {
        public int Count { get; } = (Max - Min) + 1;
    }
}
