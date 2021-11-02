using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PCRE;

namespace Shared.Parser
{
    public class CommandParser<TBase>
    {
        private readonly Dictionary<PcreRegex, Type> _types = new();

        public CommandParser<TBase> AddType<T>() => AddType(typeof(T));

        public CommandParser<TBase> AddType(Type type)
        {
            var attr = type.GetCustomAttribute<CommandRegexAttribute>();

            if (attr is null)
            {
                return this;
            }

            var regex = new PcreRegex(attr.Regex, PcreOptions.Compiled);
            _types.Add(regex, type);
            return this;
        }

        private delegate TBase Build(in PcreRefMatch.GroupList groups);

        public CommandParser<TBase> AddDerivedTypes<T>()
        {
            var types = typeof(T).Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(T));

            foreach (var type in types)
            {
                AddType(type);
            }

            return this;
        }

        public IEnumerable<TBase> ParseCommands(IInputLines lines)
            => ParseCommands(lines.AsMemory());

        public IEnumerable<TBase> ParseCommands(IEnumerable<ReadOnlyMemory<char>> lines)
        {
            var results = new List<TBase>();

            foreach (var line in lines)
            {
                foreach (var (regex, type) in _types)
                {
                    var match = regex.Match(line.Span);
                    if (match.Success)
                    {
                        var buildMethod = type.GetMethod("Build", BindingFlags.Public | BindingFlags.Static);

                        if (buildMethod is null)
                        {
                            throw new InvalidOperationException("Type requires a public build method");
                        }

                        var del = buildMethod.CreateDelegate<Build>(null);

                        results.Add(del(match.Groups));
                    }
                }
            }

            return results;
        }
    }
}
