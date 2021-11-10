namespace DayTen2016
{
    internal class Logger
    {
        private readonly List<Log> _log = new();

        public T Find<T>(Func<T, bool> filter)
        {
            return _log
                .Where(l => l is T)
                .Cast<T>()
                .First(filter);
        }

        internal void SetOutput(int bot, int value, int bucket)
        {
            _log.Add(new SetOutput(bot, value, bucket));
        }

        internal void GiveValue(int sender, int reciever, int value)
        {
            _log.Add(new GiveValue(sender, reciever, value));
        }

        internal void AssignValue(int bot, int value)
        {
            _log.Add(new AssignValue(bot, value));
        }

        internal void CompareValues(int bot, int high, int low)
        {
            _log.Add(new CompareValues(bot, high, low));
        }
    }

    record SetOutput(int Bot, int Value, int OutputBucket) : Log()
    {
        public override string ToString()
            => $"Bot {Bot} puts {Value} in output {OutputBucket}.";
    }

    record CompareValues(int Bot, int High, int Low) : Log()
    {
        public override string ToString()
            => $"Bot {Bot} compares value {Low} and {High}.";
    }

    record GiveValue(int Sender, int Reciever, int Value) : Log()
    {
        public override string ToString()
            => $"Bot {Sender} sends value-{Value} to bot {Reciever}.";
    }

    record AssignValue(int Bot, int Value) : Log()
    {
        public override string ToString()
            => $"value {Value} goes to bot {Bot}.";
    }

    record Log();
}
