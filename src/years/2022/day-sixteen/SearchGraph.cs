using Shared.Graph;

namespace DaySixteen2022;

class SearchGraph : IVertexGraph<Valve>, IWeightedGraph<Valve, byte>
{
    private readonly ImmutableDictionary<Valve, ImmutableArray<Valve>> valveMap;

    public SearchGraph(ImmutableDictionary<Valve, ImmutableArray<Valve>> valveMap)
    {
        this.valveMap = valveMap;
        Vertexes = valveMap.Keys
            .OrderBy(k => k.Name)
            .ToImmutableArray();
    }

    public ImmutableArray<Valve> Vertexes { get; }

    public byte Cost(Valve nodeA, Valve nodeB) => 1;

    public IEnumerable<Valve> Neigbours(Valve node)
        => valveMap[node];
}
