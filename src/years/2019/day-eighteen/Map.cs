using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Shared;
using Shared.Helpers;

namespace DayEighteen2019
{
    internal class Map
    {
        public Map(ImmutableDictionary<Point2d, Cell> cells)
        {
            var starts = ImmutableHashSet.CreateBuilder<Point2d>();
            var keys = ImmutableDictionary.CreateBuilder<Point2d, Key>();
            var doors = ImmutableDictionary.CreateBuilder<Point2d, Door>();
            var openspace = ImmutableHashSet.CreateBuilder<Point2d>();

            foreach (var (point, cell) in cells)
            {
                if (cell is not { CellType: CellType.Wall })
                {
                    openspace.Add(point);
                }

                switch (cell)
                {
                    case { CellType: CellType.Entrance }:
                        starts.Add(point);
                        break;
                    case Key key:
                        keys.Add(point, key);
                        break;
                    case Door door:
                        doors.Add(point, door);
                        break;
                }
            }

            Starts = starts.ToImmutable();
            Keys = keys.ToImmutable();
            Doors = doors.ToImmutable();
            OpenSpace = openspace.ToImmutable();
        }

        public ImmutableHashSet<Point2d> Starts { get; }

        public ImmutableDictionary<Point2d, Key> Keys { get; }

        public ImmutableDictionary<Point2d, Door> Doors { get; }

        public ImmutableHashSet<Point2d> OpenSpace { get; }

        public uint MinimumSteps() => MinimumSteps(Starts, ImmutableHashSet<char>.Empty, new());

        public uint MinimumSteps(
            ImmutableHashSet<Point2d> from,
            ImmutableHashSet<char> keychain,
            Dictionary<State, uint> seen)
        {
            State state = new(from, keychain);

            if (seen.TryGetValue(state, out var stored))
            {
                return stored;
            }

            var answer = FindReachableFromPoints(from, keychain)
                .Select(entry =>
                {
                    var key = entry.Key;
                    var (at, distance, started) = entry.Value;

                    var copy = from.ToBuilder();
                    copy.Remove(started);
                    copy.Add(at);

                    return distance
                        + MinimumSteps(copy.ToImmutable(), keychain.Add(key.Id), seen);
                })
                .DefaultIfEmpty()
                .Min();

            seen[state] = answer;
            return answer;
        }

        private Dictionary<Key, (Point2d At, uint Distance, Point2d Started)> FindReachableFromPoints(ISet<Point2d> from, ImmutableHashSet<char> keychain)
        {
            return new(from.SelectMany(point =>
            {
                return FindReachableKeys(point, keychain).Select(entry =>
                    KeyValuePair.Create(entry.Key, (entry.Value.KeyPoint, entry.Value.Distance, point)));
            }));
        }

        private Dictionary<Key, (Point2d KeyPoint, uint Distance)> FindReachableKeys(Point2d from, ImmutableHashSet<char> keychain)
        {
            var queue = new Queue<Point2d>();
            queue.Enqueue(from);
            var distance = new Dictionary<Point2d, uint>
            {
                [from] = 0
            };
            var keyDistiance = new Dictionary<Key, (Point2d KeyPoint, uint Distance)>();

            while (queue.TryDequeue(out var next))
            {
                foreach (var point in PointHelpers.GetDirectNeighbours(next))
                {
                    if (distance.ContainsKey(point))
                        continue;

                    if (OpenSpace.Contains(point) is false)
                        continue;

                    distance[point] = distance[next] + 1;

                    Doors.TryGetValue(point, out var door);
                    Keys.TryGetValue(point, out var key);

                    if (door is null || keychain.Contains(door.KeyId))
                    {
                        if (key is not null && keychain.Contains(key.Id) is false)
                        {
                            keyDistiance[key] = (point, distance[point]);
                        }
                        else
                        {
                            queue.Enqueue(point);
                        }
                    }
                }
            }

            return keyDistiance;
        }
    }
}
