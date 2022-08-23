using System.Security.Cryptography;
using System.Text;

namespace DayFourteen2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 14), "One-Time Pad");

        public void PartOne(IInput input, IOutput output)
        {
            var data = MD5Stream.CreateIncrementingStream(input.Content.AsString())
                .Take(100_000)
                .ToArray();

            var (index, value) = MD5Stream.Find(data)
                .Skip(63)
                .First();

            output.WriteProperty("Index", index);
            output.WriteProperty("Value", value);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var data = MD5Stream.CreateStretchedStream(input.Content.AsString())
                .Take(50_000)
                .ToArray();

            var (index, value) = MD5Stream.Find(data)
                .Skip(63)
                .First();

            output.WriteProperty("Index", index);
            output.WriteProperty("Value", value);
        }
    }

    public static class MD5Stream
    {
        private static readonly PcreRegex threeMatch = new(@"(.)\1{2}", PcreOptions.Compiled);

        public static IEnumerable<(string Value, int Index)> Find(string[] hashSource)
        {
            for (var i = 0; i < hashSource.Length; i++)
            {
                var value = hashSource[i];
                var match = threeMatch.Match(value);
                if (match.Success)
                {
                    var character = match.Groups[1].Value[0];
                    if (ValidateHash(character, hashSource.AsSpan()[(i + 1)..(i + 1001)]))
                    {
                        yield return (value, i);
                    }
                }
            }

            static bool ValidateHash(char character, ReadOnlySpan<string> span)
            {
                var target = new string(character, 5);

                foreach (var line in span)
                {
                    if (line.AsSpan().Contains(target, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static IEnumerable<string> CreateIncrementingStream(string seed)
        {
            var md5 = MD5.Create();
            for (var i = 0; ; i++)
            {
                yield return GetHexString(md5, $"{seed}{i}");
            }

            static string GetHexString(MD5 md5, string seed)
            {
                var bytes = md5.ComputeHash(Encoding.ASCII.GetBytes(seed));
                return Convert.ToHexString(bytes);
            }
        }

        public static IEnumerable<string> CreateStretchedStream(string seed)
        {
            var md5 = MD5.Create();
            for (var i = 0; ; i++)
            {
                yield return GetHexString(md5, $"{seed}{i}");
            }

            static string GetHexString(MD5 md5, string seed)
            {
                var hash = LowerHexString(md5.ComputeHash(Encoding.ASCII.GetBytes(seed)));
                for (var i = 0; i < 2016; i++)
                {
                    hash = LowerHexString(md5.ComputeHash(Encoding.ASCII.GetBytes(hash)));
                }

                return hash;
            }

            static string LowerHexString(byte[] bytes) => Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
