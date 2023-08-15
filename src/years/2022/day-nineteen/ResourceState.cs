using CommunityToolkit.HighPerformance.Helpers;

namespace DayNineteen2022;

internal readonly record struct ResourceState
{
    public delegate void UpdateFunc<in TState>(TState state, ref Builder item);

    private readonly ulong _data;

    public static readonly ResourceState Empty = new(0);

    public static ImmutableArray<Type> ResourceTypes = ImmutableArray
        .Create(Type.Ore, Type.Clay, Type.Obsidian, Type.Geode);

    public ResourceState(ulong data)
    {
        _data = data;
    }

    public ResourceState(
        ushort ore = 0,
        ushort clay = 0,
        ushort obsidian = 0,
        ushort geode = 0)
    {
        const byte size = 16;
        ulong data = 0;

        SetResourceValue(ref data, Type.Ore, ore);
        SetResourceValue(ref data, Type.Clay, clay);
        SetResourceValue(ref data, Type.Obsidian, obsidian);
        SetResourceValue(ref data, Type.Geode, geode);

        _data = data;
        return;

        static void SetResourceValue(ref ulong data, Type type, ushort value)
        {
            var index = (byte)((int)type * size);
            BitHelper.SetRange(ref data, index, size, value);
        }
    }

    public ushort Ore => GetValue(Type.Ore);

    public ushort Clay => GetValue(Type.Clay);

    public ushort Obsidian => GetValue(Type.Obsidian);

    public ushort Geode => GetValue(Type.Geode);

    public ushort this[Type type] => GetValue(type);

    public ResourceState Update<TState>(TState state, UpdateFunc<TState> action)
    {
        var builder = ToBuilder();
        action(state, ref builder);
        return builder.Build();
    }

    public Builder ToBuilder() => new(this);

    public static Builder CreateBuilder() => new();

    public IEnumerable<KeyValuePair<Type, ushort>> WithValues()
    {
        var ore = Ore;
        var clay = Clay;
        var obsidian = Obsidian;
        var geode = Geode;

        if (ore > 0)
        {
            yield return KeyValuePair.Create(Type.Ore, ore);
        }

        if (clay > 0)
        {
            yield return KeyValuePair.Create(Type.Clay, clay);
        }

        if (obsidian > 0)
        {
            yield return KeyValuePair.Create(Type.Obsidian, obsidian);
        }

        if (geode > 0)
        {
            yield return KeyValuePair.Create(Type.Geode, geode);
        }
    }

    private ushort GetValue(Type type)
    {
        var index = (byte)((int)type * 16);
        return (ushort)BitHelper.ExtractRange(_data, index, 16);
    }

    public static ResourceState operator +(ResourceState a, ResourceState b) => new(
        ore: (ushort)(a.Ore + b.Ore),
        clay: (ushort)(a.Clay + b.Clay),
        obsidian: (ushort)(a.Obsidian + b.Obsidian),
        geode: (ushort)(a.Geode + b.Geode)
    );

    public static ResourceState operator -(ResourceState a, ResourceState b) => new(
        ore: (ushort)(a.Ore - b.Ore),
        clay: (ushort)(a.Clay - b.Clay),
        obsidian: (ushort)(a.Obsidian - b.Obsidian),
        geode: (ushort)(a.Geode - b.Geode)
    );

    public static ResourceState operator *(ResourceState a, byte multiplier) => new(
        ore: (ushort)(a.Ore * multiplier),
        clay: (ushort)(a.Clay * multiplier),
        obsidian: (ushort)(a.Obsidian * multiplier),
        geode: (ushort)(a.Geode * multiplier)
    );

    public static ResourceState operator *(ResourceState a, int multiplier)
        => a * (byte)multiplier;

    public bool IntersectsWith(ResourceState other)
    {
        foreach (var type in ResourceTypes)
        {
            if (other[type] > 0 && this[type] <= 0)
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode() => _data.GetHashCode();

    public bool Equals(ResourceState other) => _data == other._data;

    internal struct Builder
    {
        public Builder()
        {
        }

        internal Builder(ResourceState data)
        {
            Ore = data.Ore;
            Clay = data.Clay;
            Obsidian = data.Obsidian;
            Geode = data.Geode;
        }

        public ushort Ore { get; set; }

        public ushort Clay { get; set; }

        public ushort Obsidian { get; set; }

        public ushort Geode { get; set; }

        public ushort this[Type type]
        {
            get => type switch
            {
                Type.Ore => Ore,
                Type.Clay => Clay,
                Type.Obsidian => Obsidian,
                Type.Geode => Geode,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            set
            {
                switch (type)
                {
                    case Type.Ore:
                        Ore = value;
                        break;
                    case Type.Clay:
                        Clay = value;
                        break;
                    case Type.Obsidian:
                        Obsidian = value;
                        break;
                    case Type.Geode:
                        Geode = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        public ResourceState Build()
        {
            return new ResourceState(
                Ore,
                Clay,
                Obsidian,
                Geode);
        }
    }
}
