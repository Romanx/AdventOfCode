using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Toolkit.HighPerformance;

namespace DayEleven2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 11), "Chronal Charge");

    public void PartOne(IInput input, IOutput output)
    {
        var serialNumber = input.Content.AsInt();
        var summedAreaTable = CalculateSummedAreaTable(300, serialNumber);

        var best = BestSquareBetweenSizes(summedAreaTable, 3, 3);

        output.WriteProperty("Serial number", serialNumber);
        output.WriteProperty($"Best {best.Size}x{best.Size} Starting at", best.TopLeft);
        output.WriteProperty("Power", best.Power);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var serialNumber = input.Content.AsInt();
        var summedAreaTable = CalculateSummedAreaTable(300, serialNumber);

        var best = BestSquareBetweenSizes(summedAreaTable, 3, 300);

        output.WriteProperty("Serial number", serialNumber);
        output.WriteProperty("Size", best.Size);
        output.WriteProperty($"Best {best.Size}x{best.Size} Starting at", best.TopLeft);
        output.WriteProperty("Power", best.Power);
    }

    private static GridSquare BestSquareBetweenSizes(int[,] grid, int min, int max)
    {
        var span = grid.AsSpan2D();

        var best = 0;
        GridSquare square = default;

        for (var size = min; size <= max; size++)
        {
            for (var y = size; y < span.Height; y++)
            {
                for (var x = size; x < span.Width; x++)
                {
                    var position = (x - size + 1, y - size + 1);
                    var power = SumOfSquare(grid, x, y, size);
                    if (power > best)
                    {
                        best = power;
                        square = new GridSquare(position, power, size);
                    }
                }
            }
        }

        return square;
    }

    /// <summary>
    /// Build a summed area table which represents a cascading sum across all cells before this point
    /// NxM
    /// </summary>
    /// <param name="size">The size of the grid to generate</param>
    /// <param name="serialNumber">The serial number to use to populate the initial value</param>
    /// <returns></returns>
    private static int[,] CalculateSummedAreaTable(int size, int serialNumber)
    {
        var grid = new int[size, size];
        var span = grid.AsSpan2D();
        for (var y = 0; y < span.Height; y++)
        {
            for (var x = 0; x < span.Width; x++)
            {
                var current = CaclulatePowerLevel(x, y, serialNumber);
                var up = y == 0 ? 0 : grid[y - 1, x];
                var left = x == 0 ? 0 : grid[y, x - 1];
                var upperLeft = (x == 0 || y == 0) ? 0 : grid[y - 1, x - 1];
                var total = current + up + left - upperLeft;
                grid[y, x] = total;
            }
        }

        return grid;

        static int CaclulatePowerLevel(int x, int y, int serialNumber)
        {
            var rackId = x + 10;
            var powerLevel = ((rackId * y) + serialNumber) * rackId;

            return ((powerLevel / 100) % 10) - 5;
        }
    }

    /// <summary>
    /// Find the sum of the specific square size we want.
    /// This is complicated by the fact that we need to subtract the numbers that came before us and add in some numbers we'll over remove.
    /// https://todd.ginsberg.com/post/advent-of-code/2018/day11/ has a good visual example of this
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private static int SumOfSquare(int[,] grid, int x, int y, int size)
    {
        var lowerRight = grid[y, x];
        var upperRight = y - size >= 0 ? grid[y - size, x] : 0;
        var lowerLeft = x - size >= 0 ? grid[y, x - size] : 0;
        var upperLeft = (x - size >= 0 && y - size >= 0) ? grid[y - size, x - size] : 0;

        return lowerRight + upperLeft - upperRight - lowerLeft;
    }
}

readonly record struct GridSquare(Point2d TopLeft, int Power, int Size);
