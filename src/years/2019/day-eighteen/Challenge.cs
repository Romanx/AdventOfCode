using CommunityToolkit.HighPerformance;

namespace DayEighteen2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 18), "Many-Worlds Interpretation");

        public void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();
            var result = map.MinimumSteps();

            output.WriteProperty("Number of steps", result);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var map = ConvertMap(input);
            var result = map.MinimumSteps();

            output.WriteProperty("Number of steps", result);

            static Map ConvertMap(IInput input)
            {
                var replacement = new char[,]
                {
                    {  '@', '#', '@' },
                    {  '#', '#', '#' },
                    {  '@', '#', '@' },
                }.AsSpan2D();

                var entrances = new HashSet<Point2d>();

                var array = input.Lines.As2DArray();
                var xLength = array.GetLength(0);
                var yLength = array.GetLength(1);
                for (var y = 0; y < yLength; y++)
                {
                    for (var x = 0; x < xLength; x++)
                    {
                        if (array[x, y] == '@')
                        {
                            entrances.Add(new(x, y));
                            break;
                        }
                    }
                }

                if (entrances.Count > 1)
                {
                    return array.Parse();
                }
                var entrance = entrances.First();

                var arraySpan = array.AsSpan2D();
                var entranceArea = arraySpan[
                    (entrance.X - 1)..(entrance.X + 2),
                    (entrance.Y - 1)..(entrance.Y + 2)];

                replacement.CopyTo(entranceArea);
                return array.Parse();
            }
        }
    }

    enum CellType { Wall, Key, Door, Entrance, Empty }

    record Cell(CellType CellType, Point2d Point);

    record Key(char Id, Point2d Point) : Cell(CellType.Key, Point);

    record Door(char DoorId, char KeyId, Point2d Point) : Cell(CellType.Door, Point);

    class State : IEquatable<State?>
    {
        public State(ImmutableHashSet<Point2d> from, ImmutableHashSet<char> keychain)
        {
            From = from;
            Keychain = keychain;
        }

        public ImmutableHashSet<Point2d> From { get; }

        public ImmutableHashSet<char> Keychain { get; }

        public bool Equals(State? other)
        {
            return other is not null
                && From.SetEquals(other.From)
                && Keychain.SetEquals(other.Keychain);
        }

        public override bool Equals(object? obj) => Equals(obj as State);

        public override int GetHashCode()
        {
            HashCode hashcode = default;
            foreach (var item in From)
                hashcode.Add(item);

            foreach (var item in Keychain)
                hashcode.Add(item);

            return hashcode.ToHashCode();
        }
    }
}
