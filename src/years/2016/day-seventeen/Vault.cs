using System.Security.Cryptography;
using System.Text;
using Shared.Graph;
using Shared.Grid;

namespace DaySeventeen2016
{
    class Vault : IGraph<string>
    {
        private readonly MD5 _hasher;
        private readonly Area2d _area;
        private static readonly char[] open = new char[] { 'B', 'C', 'D', 'E', 'F' };
        private readonly string _passcode;

        public Vault(string passcode)
        {
            _hasher = MD5.Create();
            _area = Area2d.Create(Point2d.Origin, new(3, 3));
            _passcode = passcode;
        }

        public IEnumerable<string> Search(string start)
        {
            var goal = new Point2d(3, 3);
            var currentFrontier = new List<string>();
            var nextFrontier = new List<string>();
            currentFrontier.Add(start);

            while (currentFrontier.Count > 0)
            {
                foreach (var current in currentFrontier)
                {
                    var currentPosition = CalculatePosition(current);

                    if (currentPosition == goal)
                    {
                        yield return current;
                        continue;
                    }

                    foreach (var next in Neighbours(current))
                    {
                        nextFrontier.Add(next);
                    }
                }

                (currentFrontier, nextFrontier) = (nextFrontier, currentFrontier);
                nextFrontier.Clear();
            }
        }

        public IEnumerable<string> Neighbours(string path)
        {
            var hash = Convert.ToHexString(_hasher.ComputeHash(Encoding.ASCII.GetBytes(_passcode + path)));

            var directions = hash.AsSpan()[0..4];
            var neighbours = new List<string>(4);

            var position = CalculatePosition(path);

            for (var i = 0; i < directions.Length; i++)
            {
                var id = DirectionIdentifier(i);
                var direction = GridDirection.FromChar(id);
                var target = position + direction;

                if (open.Contains(directions[i]) && _area.Contains(target))
                {
                    neighbours.Add(path + id);
                }
            }

            return neighbours;
        }

        private static Point2d CalculatePosition(string path)
        {
            var position = Point2d.Origin;
            foreach (var c in path)
            {
                position += GridDirection.FromChar(c);
            }

            return position;
        }

        private static char DirectionIdentifier(int index)
        {
            return index switch
            {
                0 => 'U',
                1 => 'D',
                2 => 'L',
                3 => 'R',
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
