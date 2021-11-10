namespace DayThree2016
{
    internal static class ParseExtensions
    {
        public static int[,] Parse(this IInput input)
        {
            var lines = input.Lines.AsArray();

            var grid = new int[lines.Length, 3];

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var split = line
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => int.Parse(i))
                    .ToArray();

                for (var y = 0; y < split.Length; y++)
                {
                    grid[i, y] = split[y];
                }

            }

            return grid;
        }
    }
}
