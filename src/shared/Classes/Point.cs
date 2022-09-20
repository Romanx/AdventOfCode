using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using Shared.Helpers;

namespace Shared
{
    public record Point
    {
        public ImmutableArray<int> Dimensions { get; }

        protected Point(ImmutableArray<int> dimensions) => Dimensions = dimensions;

        public int this[int index] => Dimensions[index];

        public override int GetHashCode()
        {
            HashCode code = default;
            foreach (var dimension in Dimensions)
            {
                code.Add(dimension);
            }

            return code.ToHashCode();
        }

        public virtual bool Equals(Point? other) => other is not null
            && other.Dimensions.SequenceEqual(Dimensions);

        public override string ToString() => $"[{string.Join(", ", Dimensions)}]";

        public static ImmutableArray<int> ConvertToPoint(IEnumerable<int> dimensionValues, int dimensionCount)
        {
            var arr = dimensionValues.ToArray();

            if (arr.Length == dimensionCount)
            {
                return arr.ToImmutableArray();
            }

            if (dimensionCount > arr.Length)
            {
                var builder = ImmutableArray.CreateBuilder<int>(dimensionCount);
                builder.AddRange(arr);
                for (var i = arr.Length; i < dimensionCount; i++)
                {
                    builder.Add(0);
                }

                return builder.ToImmutable();
            }
            else
            {
                return dimensionValues.Slice(0, dimensionCount).ToImmutableArray();
            }
        }
    }
}
