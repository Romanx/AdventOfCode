namespace Shared
{
    public abstract record Command(string Name, object[] Arguments)
    {
        public abstract void Apply(Program program);

        public static object[] ParseArguments(in PcreRefMatch.GroupList groups)
        {
            var args = new object[groups.Count - 1];

            for (var i = 1; i < groups.Count; i++)
            {
                var value = groups[i].Value;

                args[i - 1] = int.TryParse(value, out var res)
                    ? (object)res
                    : new Register(value[0]);
            }

            return args;
        }
    }

    public readonly struct Register
    {
        public Register(char registerName)
        {
            RegisterName = registerName;
        }

        public char RegisterName { get; }

        public static implicit operator char(Register r) => r.RegisterName;

        public override string ToString() => $"{RegisterName}";
    }
}
