
namespace DayTen2016
{
    class Factory
    {
        private readonly Dictionary<int, int> _outputs;
        private readonly Dictionary<int, Bot> _bots;

        public Logger Logger { get; }

        public ImmutableDictionary<int, int> Outputs => _outputs.ToImmutableDictionary();

        public Factory()
        {
            _outputs = new Dictionary<int, int>();
            _bots = new Dictionary<int, Bot>();
            Logger = new Logger();
        }

        internal void ExecuteInstructions(ImmutableList<Action> actions)
        {
            var assignments = actions.Where(a => a is Assignment).Cast<Assignment>();

            AssignBots(assignments, _bots, Logger);

            actions = actions.RemoveRange(assignments);

            var transfers = actions.Cast<Transfer>().ToImmutableList();
            while (transfers.Count > 0)
            {
                var readyBot = _bots.First(b => b.Value.ReadyForTransfer).Value;
                var instruction = transfers.Find(a => a.BotNumber == readyBot.Number)!;
                var (high, low) = readyBot.GetValues(Logger);

                ApplyTarget(readyBot, instruction.LowTarget, low, _outputs, _bots, Logger);
                ApplyTarget(readyBot, instruction.HighTarget, high, _outputs, _bots, Logger);
                transfers = transfers.Remove(instruction);
            }

            static void AssignBots(IEnumerable<Assignment> assignments, Dictionary<int, Bot> bots, Logger logger)
            {
                foreach (var assignment in assignments)
                {
                    ref var bot = ref bots.GetOrAddValueRef(assignment.BotNumber);
                    bot ??= new Bot(assignment.BotNumber);

                    bot.Set(assignment.Value);
                    logger.AssignValue(bot.Number, assignment.Value);
                }
            }

            static void ApplyTarget(
                Bot readyBot,
                Target target,
                int value,
                Dictionary<int, int> outputs,
                Dictionary<int, Bot> bots,
                Logger logger)
            {
                if (target.TargetType is TargetType.Output)
                {
                    outputs[target.Number] = value;

                    logger.SetOutput(readyBot.Number, value, target.Number);
                }
                else
                {
                    ref var targetBot = ref bots.GetOrAddValueRef(target.Number);
                    targetBot ??= new Bot(target.Number);

                    targetBot.Set(value);

                    logger.GiveValue(readyBot.Number, target.Number, value);
                }
            }
        }
    }
}
