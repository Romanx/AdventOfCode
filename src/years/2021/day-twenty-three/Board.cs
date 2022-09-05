using CommunityToolkit.HighPerformance;

namespace DayTwentyThree2021;

readonly struct Board : IEquatable<Board>
{
    private static readonly ImmutableDictionary<Amphipod, int> targetRooms = new Dictionary<Amphipod, int>()
    {
        [Amphipod.Amber] = 2,
        [Amphipod.Bronze] = 4,
        [Amphipod.Copper] = 6,
        [Amphipod.Desert] = 8,
    }.ToImmutableDictionary();

    private static readonly ImmutableArray<int> validHallwayLocations = ImmutableArray.Create(0, 1, 3, 5, 7, 9, 10);

    public Board(string hallway, ImmutableDictionary<Amphipod, Room> rooms)
    {
        Hallway = hallway;
        Rooms = rooms;
        Organized = rooms.Values.All(room => room.Organized);
    }

    public bool Organized { get; }

    public string Hallway { get; }

    public ImmutableDictionary<Amphipod, Room> Rooms { get; }

    public IEnumerable<(Board Board, int Cost)> NextStates()
    {
        foreach (var (index, room) in FindAmphipodsInHallway(this))
        {
            var y = room.Content.LastIndexOf('.') + 1;
            var cost = (Math.Abs(index - room.Index) + y) * room.Amphipod.MovementMultiplier;

            var next = new Board(Hallway.ReplaceCharAt(index, '.'), Rooms.SetItem(room.Amphipod, room.Add()));
            yield return (next, cost);
        }

        foreach (var room in Rooms.Values.Where(room => room.HasAmpipodsOfIncorrectType))
        {
            var (amphipod, index) = room.RetrieveFirst();
            foreach (var hallwayIndex in CurrentValidHallwayLocations(Hallway, room.Index))
            {
                var y = index + 1;
                var cost = (Math.Abs(hallwayIndex - room.Index) + y) * amphipod.MovementMultiplier;

                var updatedRoom = new Room(room.Amphipod, room.Index, room.Content.ReplaceCharAt(index, '.'));
                var next = new Board(Hallway.ReplaceCharAt(hallwayIndex, amphipod.Type), Rooms.SetItem(room.Amphipod, updatedRoom));
                yield return (next, cost);
            }
        }

        static IEnumerable<(int Index, Room Target)> FindAmphipodsInHallway(Board board)
        {
            for (var i = 0; i < board.Hallway.Length; i++)
            {
                var c = board.Hallway[i];
                if (c is not '.')
                {
                    var room = board.Rooms[Amphipod.Parse(c)];
                    if (room.IsEmptyOrHasAllValidAmphipods && HallwayPathClear(board.Hallway, i, room.Index))
                    {
                        yield return (i, room);
                    }
                }
            }
        }

        static bool HallwayPathClear(string hallway, int start, int end)
        {
            var span = hallway.AsSpan();

            var slice = start > end
                ? span[end..start]
                : span[(start + 1)..(end + 1)];

            foreach (var c in slice)
            {
                if (c is not '.')
                {
                    return false;
                }
            }

            return true;
        }

        static IEnumerable<int> CurrentValidHallwayLocations(string hallway, int end)
        {
            foreach (var possible in validHallwayLocations)
            {
                if (hallway[possible] is '.' && HallwayPathClear(hallway, possible, end))
                {
                    yield return possible;
                }
            }
        }
    }

    public override string ToString()
    {
        return $"{Hallway}|{string.Join('|', Rooms.Values.OrderBy(r => r.Index))}";
    }

    public static Board Parse(char[,] board)
    {
        var span = board.AsSpan2D();
        var hallway = span.GetRowSpan(1)[1..^1];
        var rooms = span[2..^1, 1..];

        var builder = ImmutableDictionary.CreateBuilder<Amphipod, Room>();
        foreach (var (type, index) in targetRooms)
        {
            var contents = new string(rooms.GetColumn(index).ToArray());
            builder.Add(type, new Room(type, index, contents));
        }

        return new Board(new string(hallway), builder.ToImmutable());
    }

    public override int GetHashCode() => ToString().GetHashCode();

    public bool Equals(Board other)
        => ToString() == other.ToString();

    public override bool Equals(object? obj) => obj is Board board && Equals(board);
}
