namespace DayTwentyTwo2015
{
    delegate (GameState NextState, Result? Result) StateTransition(GameState state);

    class Game
    {
        private readonly Wizard _wizard;
        private readonly Boss _boss;
        private readonly GamePipelineBuilder _pipelineBuilder;

        public Game(Wizard wizard, Boss boss, bool hardMode = false)
        {
            _wizard = wizard;
            _boss = boss;
            _pipelineBuilder = new GamePipelineBuilder(hardMode);
        }

        public GameState FindBestRun()
        {
            var queue = new Queue<GameState>();
            queue.Enqueue(new GameState(_wizard, _boss));

            GameState? best = null;

            while (queue.TryDequeue(out var state))
            {
                if (best is not null && state.TotalManaSpent >= best.TotalManaSpent)
                    continue;

                foreach (var spell in SpellBook.Spells)
                {
                    var (newState, result) = TakeTurn(state, spell);

                    if (result == Result.Invalid)
                    {
                        continue;
                    }
                    else if (result == Result.Continue)
                    {
                        queue.Enqueue(newState);
                    }
                    else if (result == Result.WizardWin)
                    {
                        if (best is null || newState.TotalManaSpent < best.TotalManaSpent)
                        {
                            best = newState;
                        }
                    }
                }
            }

            return best!;
        }

        private (GameState NextState, Result Result) TakeTurn(GameState state, SpellBase spell)
        {
            Result? result;
            foreach (var step in _pipelineBuilder.GetPipeline(spell))
            {
                (state, result) = step(state);

                if (result is not null)
                {
                    return (state, result.Value);
                }
            }

            return (state, Result.Continue);
        }
    }
}
