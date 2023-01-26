using System.Collections.Immutable;

namespace Shared;

public static class InfiniteQueue
{
    public static InfiniteQueue<TItem> Create<TItem>(ImmutableArray<TItem> items)
        => new(items);
}

public sealed class InfiniteQueue<T>(ImmutableArray<T> items)
{
    private readonly ImmutableArray<T> items = items;

    public int Index { get; private set; }

    public T Dequeue()
    {
        var item = items[Index];
        Index = (Index + 1) % items.Length;
        return item;
    }
}
