using System.Text.RegularExpressions;

namespace DaySeven2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 7), "Internet Protocol Version 7");

        private static readonly Regex abbaRegex = new(@"([a-z])((?!\1)[a-z])\2\1");
        private static readonly Regex abaRegex = new(@"([a-z])((?!\1)[a-z])\1");
        private static readonly Regex hypernetRegex = new(@"\[[a-z]+\]");

        public override void PartOne(IInput input, IOutput output)
        {
            var validAddresses = input.Lines.AsString()
                .Count(static ip => ValidateIp(ip).Tls);

            output.WriteProperty("Number of valid addresses", validAddresses);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var validAddresses = input.Lines.AsString()
                .Count(static ip => ValidateIp(ip).Ssl);

            output.WriteProperty("Number of valid addresses", validAddresses);
        }

        private static (bool Tls, bool Ssl) ValidateIp(string line)
        {
            var hypernetList = new List<string>();
            var supernet = hypernetRegex.Replace(line, (match) =>
            {
                hypernetList.Add(match.Groups[0].Value);
                return "--";
            });

            var hypernet = string.Join("--", hypernetList);

            var tls = Tls(supernet, hypernet);
            var ssl = Ssl(supernet, hypernet);

            return (tls, ssl);

            static bool Tls(string supernet, string hypernet)
            {
                return abbaRegex.IsMatch(supernet) is true &&
                       abbaRegex.IsMatch(hypernet) is false;
            }

            static bool Ssl(string supernet, string hypernet)
            {
                foreach (var match in abaRegex.MatchesOverlapped(supernet))
                {
                    var a = match.Groups[1].Value;
                    var b = match.Groups[2].Value;

                    if (hypernet.Contains($"{b}{a}{b}"))
                        return true;
                }

                return false;
            }
        }
    }
}
