using System;

namespace Shared.HexGrid
{
    public record class Hex(Point3d Point)
    {
        public static Hex Origin { get; } = new Hex(Point3d.Origin);

        public static Hex operator +(Hex hex, HexDirection direction)
        {
            var newPoint = direction.DirectionType switch
            {
                HexDirectionType.East => hex.Point + (1, -1, 0),
                HexDirectionType.SouthEast => hex.Point + (0, -1, 1),
                HexDirectionType.SouthWest => hex.Point + (-1, 0, 1),
                HexDirectionType.West => hex.Point + (-1, 1, 0),
                HexDirectionType.NorthWest => hex.Point + (0, 1, -1),
                HexDirectionType.NorthEast => hex.Point + (1, 0, -1),
                _ => throw new NotImplementedException(),
            };

            return new(newPoint);
        }
    }

    public record HexDirection(HexDirectionType DirectionType)
    {
        public static HexDirection East { get; } = new HexDirection(HexDirectionType.East);
        public static HexDirection SouthEast { get; } = new HexDirection(HexDirectionType.SouthEast);
        public static HexDirection SouthWest { get; } = new HexDirection(HexDirectionType.SouthWest);
        public static HexDirection West { get; } = new HexDirection(HexDirectionType.West);
        public static HexDirection NorthWest { get; } = new HexDirection(HexDirectionType.NorthWest);
        public static HexDirection NorthEast { get; } = new HexDirection(HexDirectionType.NorthEast);

        public static HexDirection Parse(string str)
        {
            return str.Trim().ToUpper() switch
            {
                "E" => East,
                "SE" => SouthEast,
                "SW" => SouthWest,
                "W" => West,
                "NE" => NorthEast,
                "NW" => NorthWest,
                _ => throw new NotImplementedException()
            };
        }
    }


    public enum HexDirectionType
    {
        East,
        SouthEast,
        SouthWest,
        West,
        NorthWest,
        NorthEast
    }
}
