namespace DayTwentyThree2015
{
    class Program
    {
        private readonly ImmutableArray<Command> _commands;

        public Program(ImmutableArray<Command> commands)
        {
            _commands = commands;
        }

        public ExecutionState Run(Dictionary<string, uint> registers)
        {
            var es = new ExecutionState(0, registers.ToImmutableDictionary());

            while (es.Pointer < _commands.Length)
            {
                var command = _commands[es.Pointer];
                es = command.Run(es);
            }

            return es;
        }

        public ExecutionState Run()
            => Run(new Dictionary<string, uint>
            {
                ["a"] = 0,
                ["b"] = 0,
            });
    }
}
