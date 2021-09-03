using System;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.ObjectPool;
using NodaTime;
using Shared;

namespace DayEight2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 8), "Matchsticks");

        public override void PartOne(IInput input, IOutput output)
        {
            var total = 0;
            var converted = 0;
            var decoder = new Decoder();

            foreach (var line in input.AsStringLines())
            {
                total += line.Length;
                var decoded = decoder.Decode(line);
                converted += decoded.Length;
            }

            output.WriteProperty("Number of characters", total);
            output.WriteProperty("Number of decoded characters", converted);
            output.WriteProperty("Difference in characters", total - converted);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var total = 0;
            var encodedTotal = 0;
            var encoder = new Encoder();

            foreach (var line in input.AsStringLines())
            {
                total += line.Length;

                var encoded = encoder.Encode(line);
                encodedTotal += encoded.Length;
            }

            output.WriteProperty("Number of characters", total);
            output.WriteProperty("Number of encoding characters", encodedTotal);
            output.WriteProperty("Difference in characters", encodedTotal - total);
        }

        private class Decoder
        {
            public string Decode(string input)
            {
                return Regex.Replace(
                    input.Trim('"')
                    .Replace("\\\"", "A")
                    .Replace("\\\\", "B"), "\\\\x[a-f0-9]{2}", "C");
            }
        }

        private class Encoder
        {
            private readonly ObjectPool<StringBuilder> stringBuilderPool;

            public Encoder()
            {
                var objectPoolProvider = new DefaultObjectPoolProvider();
                stringBuilderPool = objectPoolProvider.CreateStringBuilderPool();
            }

            public string Encode(string input)
            {
                var builder = stringBuilderPool.Get();
                try
                {
                    builder.Append(input);

                    return builder
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Insert(0, '"')
                        .Insert(builder.Length, '"')
                        .ToString();
                }
                finally
                {
                    stringBuilderPool.Return(builder);
                }
            }
        }
    }
}
