namespace DaySixteen2022;

readonly record struct SearchState(
    ImmutableArray<Valve> CurrentPositions,
    uint CurrentTime,
    uint TotalPressure,
    ImmutableHashSet<string> OpenValves) : IEquatable<SearchState>
{
    public SearchState OpenValve(Valve valve, uint currentTime, uint maxTime)
    {
        return this with
        {
            CurrentTime = CurrentTime + 1,
            OpenValves = OpenValves.Add(valve.Name),
            TotalPressure = TotalPressure + (valve.FlowRate * (maxTime - currentTime))
        };
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        foreach (var position in CurrentPositions)
        {
            hash.Add(position);
        }
        hash.Add(TotalPressure);
        foreach (var valve in OpenValves)
        {
            hash.Add(valve);
        }

        return hash.ToHashCode();
    }

    public bool Equals(SearchState other)
    {
        return other.TotalPressure == TotalPressure
            && CurrentPositions.SequenceEqual(other.CurrentPositions)
            && OpenValves.SequenceEqual(other.OpenValves);
    }
}
