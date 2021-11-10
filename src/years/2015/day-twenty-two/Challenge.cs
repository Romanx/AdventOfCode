using Spectre.Console;

namespace DayTwentyTwo2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 22), "Wizard Simulator 20XX");

        public override void PartOne(IInput input, IOutput output)
        {
            var wizard = new Wizard(50, 0, 500);
            var boss = input.Parse();
            var game = new Game(wizard, boss);
            var state = game.FindBestRun();

            output.WriteProperty("Least amount of mana spent", state.TotalManaSpent);
            output.WriteBlock(() =>
            {
                var body = string.Join(Environment.NewLine, state.Log).Trim();
                return new Panel(body)
                {
                    Header = new PanelHeader("Battle Log")
                };
            });
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var wizard = new Wizard(50, 0, 500);
            var boss = input.Parse();
            var game = new Game(wizard, boss, hardMode: true);
            var state = game.FindBestRun();

            output.WriteProperty("Least amount of mana spent", state.TotalManaSpent);
            output.WriteBlock(() =>
            {
                var body = string.Join(Environment.NewLine, state.Log).Trim();
                return new Panel(body)
                {
                    Header = new PanelHeader("Battle Log")
                };
            });
        }
    }

    internal static class ParseExtensions
    {
        public static Boss Parse(this IInput input)
        {
            var lines = input.Lines.AsArray();

            return new Boss(
                int.Parse(lines[0].Split(':')[1]),
                int.Parse(lines[1].Split(':')[1]));
        }
    }
}
