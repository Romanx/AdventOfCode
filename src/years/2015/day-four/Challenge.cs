using System;
using System.Security.Cryptography;
using System.Text;
using NodaTime;
using Shared;

namespace DayFour2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 4), "The Ideal Stocking Stuffer");

        public override void PartOne(IInput input, IOutput output)
        {
            var key = input.AsString();

            var (adventCoin, inputHash, outputHash) = CalculateAdventCoin(key, "00000");

            output.WriteProperty("Hash Input", inputHash);
            output.WriteProperty("Hash Output", outputHash);
            output.WriteProperty("Advent Coin", adventCoin);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var key = input.AsString();

            var (adventCoin, inputHash, outputHash) = CalculateAdventCoin(key, "000000");

            output.WriteProperty("Hash Input", inputHash);
            output.WriteProperty("Hash Output", outputHash);
            output.WriteProperty("Advent Coin", adventCoin);
        }

        private static (int AdventCoin, string HashInput, string HashOutput) CalculateAdventCoin(string key, string targetPrefix)
        {
            using var md5 = MD5.Create();
            string hashInput;
            string hashOutput;
            int i;
            for (i = 0; ; i++)
            {
                hashInput = $"{key}{i}";
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(hashInput));

                hashOutput = Convert.ToHexString(result);
                if (hashOutput.StartsWith(targetPrefix))
                {
                    break;
                }
            }

            return (i, hashInput, hashOutput);
        }
    }

    internal static class ParseExtensions
    {
    }
}
