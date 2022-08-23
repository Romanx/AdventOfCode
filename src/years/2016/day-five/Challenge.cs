using System.Security.Cryptography;
using System.Text;

namespace DayFive2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 5), "How About a Nice Game of Chess?");

        public void PartOne(IInput input, IOutput output)
        {
            var password = GeneratePassword(input.Content.AsString());

            output.WriteProperty("Password", password);

            static string GeneratePassword(string doorId)
            {
                Span<char> password = new char[8];
                var expectedStart = new char[] { '0', '0', '0', '0', '0' };
                var index = 0;
                var md5 = MD5.Create();

                for (var i = 1; index < password.Length; i++)
                {
                    var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(doorId + i));
                    var hexString = Convert.ToHexString(hash)
                        .AsSpan();
                    if (hexString[..5].SequenceEqual(expectedStart))
                    {
                        password[index] = hexString[5];
                        index++;
                    }
                }

                return new string(password);
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var password = GeneratePassword(input.Content.AsString());

            output.WriteProperty("Password", password);

            static string GeneratePassword(string doorId)
            {
                Span<char> password = new char[8];
                var expectedStart = new char[] { '0', '0', '0', '0', '0' };
                var md5 = MD5.Create();

                for (var i = 1; ; i++)
                {
                    var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(doorId + i));
                    var hexString = Convert.ToHexString(hash)
                        .AsSpan();

                    if (hexString[..5].SequenceEqual(expectedStart))
                    {
                        if (int.TryParse($"{hexString[5]}", out var position) is false)
                        {
                            continue;
                        }

                        if (position > 7)
                            continue;

                        var character = hexString[6];
                        if (password[position] == 0)
                        {
                            password[position] = character;

                            if (password.Contains((char)0) is false)
                            {
                                break;
                            }
                        }
                    }
                }

                return new string(password);
            }
        }
    }

    internal static class ParseExtensions
    {
    }
}
