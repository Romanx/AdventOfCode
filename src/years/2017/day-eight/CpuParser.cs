using Shared.Parser;

namespace DayEight2017
{
    static class CpuParser
    {
        public static CommandParser<Instruction> BuildParser()
        {
            var parser = new CommandParser<Instruction>();

            var types = typeof(Instruction).Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(Instruction));

            foreach (var type in types)
            {
                parser.AddType(type);
            }

            return parser;
        }
    }

    abstract record Instruction(string Name)
    {
        public abstract void Apply(Dictionary<string, int> registry);
    }

    [CommandRegex("(?<Registry>[a-z]+) inc (?<OpValue>-?[0-9]*) if (?<ConditionRegistry>[a-z]+) (?<ConditionOperator>>|<|>=|==|!=|<=) (?<ConditionValue>-?[0-9]*)")]
    record Increment(string Registry, int OpValue, RegistryCondition Condition) : Instruction("inc")
    {
        public static Increment Build(ref PcreRefMatch.GroupList groups)
        {
            var registry = new string(groups["Registry"].Value);
            var opValue = int.Parse(groups["OpValue"].Value);
            var condition = RegistryCondition.Parse(ref groups);

            return new Increment(registry, opValue, condition);
        }

        public override void Apply(Dictionary<string, int> registry)
        {
            if (Condition.Check(registry))
            {
                ref var registryValue = ref registry.GetOrAddValueRef(Registry);
                registryValue += OpValue;
            }
        }
    }

    [CommandRegex("(?<Registry>[a-z]+) dec (?<OpValue>-?[0-9]*) if (?<ConditionRegistry>[a-z]+) (?<ConditionOperator>>|<|>=|==|!=|<=) (?<ConditionValue>-?[0-9]*)")]
    record Decrement(string Registry, int OpValue, RegistryCondition Condition) : Instruction("dec")
    {
        public static Decrement Build(ref PcreRefMatch.GroupList groups)
        {
            var registry = new string(groups["Registry"].Value);
            var opValue = int.Parse(groups["OpValue"].Value);
            var condition = RegistryCondition.Parse(ref groups);

            return new Decrement(registry, opValue, condition);
        }

        public override void Apply(Dictionary<string, int> registry)
        {
            if (Condition.Check(registry))
            {
                ref var registryValue = ref registry.GetOrAddValueRef(Registry);
                registryValue -= OpValue;
            }
        }
    }

    record RegistryCondition(string Registry, string Operator, int ConditionValue)
    {
        public static RegistryCondition Parse(ref PcreRefMatch.GroupList groups)
        {
            var registry = new string(groups["ConditionRegistry"].Value);
            var op = new string(groups["ConditionOperator"].Value);
            var value = int.Parse(groups["ConditionValue"].Value);

            return new RegistryCondition(registry, op, value);
        }

        public bool Check(Dictionary<string, int> registry)
        {
            var value = registry.TryGetValue(Registry, out var regVal)
                ? regVal
                : 0;

            return Operator switch
            {
                ">" => value > ConditionValue,
                "<" => value < ConditionValue,
                "==" => value == ConditionValue,
                "!=" => value != ConditionValue,
                ">=" => value >= ConditionValue,
                "<=" => value <= ConditionValue,
                _ => throw new System.NotImplementedException("Operator has not been implemented."),
            };
        }
    }
}
