using System;
using System.Collections.Immutable;
using System.Linq;
using Shared;

namespace DayTwentyFour2020
{
    internal static class ParseExtensions
    {
        public static ImmutableArray<ImmutableArray<HexDirection>> ParsePaths(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<ImmutableArray<HexDirection>>();

            foreach (var line in input.AsLines())
            {
                builder.Add(ParsePath(line));
            }

            return builder.ToImmutable();

            static ImmutableArray<HexDirection> ParsePath(ReadOnlyMemory<char> line)
            {
                var builder = ImmutableArray.CreateBuilder<HexDirection>();
                var span = line.Span;
                while (span.IsEmpty is false)
                {
                    HexDirection direction;
                    if (span.Length >= 2 && span[..2].SequenceEqual("se"))
                    {
                        direction = HexDirection.SouthEast;
                        span = span[2..];
                    }
                    else if (span.Length >= 2 && span[..2].SequenceEqual("sw"))
                    {
                        direction = HexDirection.SouthWest;
                        span = span[2..];
                    }
                    else if (span.Length >= 2 && span[..2].SequenceEqual("nw"))
                    {
                        direction = HexDirection.NorthWest;
                        span = span[2..];
                    }
                    else if (span.Length >= 2 && span[..2].SequenceEqual("ne"))
                    {
                        direction = HexDirection.NorthEast;
                        span = span[2..];
                    }
                    else if (span.Length >= 1 && span[0] == 'e')
                    {
                        direction = HexDirection.East;
                        span = span[1..];
                    }
                    else if (span.Length >= 1 && span[0] == 'w')
                    {
                        direction = HexDirection.West;
                        span = span[1..];
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unable to work out a direction from {span[..2].ToString()}");
                    }

                    builder.Add(direction);
                }

                return builder.ToImmutable();
            }
        }
    }
}
