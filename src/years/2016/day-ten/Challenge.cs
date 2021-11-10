namespace DayTen2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 10), "Balance Bots");

        public override void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Parse();

            var factory = new Factory();
            factory.ExecuteInstructions(instructions);

            var targetLog = factory.Logger.Find<CompareValues>(cv =>
            {
                return cv.High == 61 && cv.Low == 17; ;
            });

            output.WriteProperty("Bot Number", targetLog.Bot);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Parse();

            var factory = new Factory();
            factory.ExecuteInstructions(instructions);

            var factoryOutputs = factory.Outputs;
            var one = factoryOutputs[0];
            var two = factoryOutputs[1];
            var three = factoryOutputs[2];

            output.WriteProperty("Output One", one);
            output.WriteProperty("Output Two", two);
            output.WriteProperty("Output Three", three);
            output.WriteProperty("Result", one * two * three);
        }
    }

    class Bot
    {
        private int? _first;
        private int? _second;

        public Bot(int number)
        {
            Number = number;
        }

        public int Number { get; }

        public bool ReadyForTransfer =>
            _first is not null &&
            _second is not null;

        public (int High, int Low) GetValues(Logger logger)
        {
            if (ReadyForTransfer is false)
                throw new InvalidOperationException("Can't get values if both aren't assigned");

            var high = Math.Max(_first!.Value, _second!.Value);
            var low = Math.Min(_first.Value, _second.Value);
            logger.CompareValues(Number, high, low);

            _first = null;
            _second = null;

            return (high, low);
        }

        internal void Set(int value)
        {
            if (ReadyForTransfer)
                throw new InvalidOperationException("No empty value to set");


            if (_first is null)
            {
                _first = value;
            }
            else
            {
                _second = value;
            }
        }
    }

    enum TargetType
    {
        Bot,
        Output
    }
}
