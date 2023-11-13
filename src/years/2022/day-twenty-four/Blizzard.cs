using CommunityToolkit.HighPerformance;

namespace DayTwentyFour2022;

readonly record struct Blizzard(
    Point2d Position,
    Point2d Start,
    Point2d End,
    Direction Facing)
{
    public Blizzard Move(ReadOnlySpan2D<char> map)
    {
        var next = Position + Facing;
        var nextValue = map.At(next);

        // We've hit a wall so we must be at the end
        if (nextValue is '#')
        {
            if (Position == End)
            {
                return new(Start, Start, End, Facing);
            }
            else if (Position == Start)
            {
                return new(End, Start, End, Facing);
            }

            throw new InvalidOperationException("Well something went wrong");
        }

        return new(next, Start, End, Facing);
    }
}
