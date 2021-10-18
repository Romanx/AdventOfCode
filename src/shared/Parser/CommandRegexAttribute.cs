using System;

namespace Shared.Parser
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CommandRegexAttribute : Attribute
    {
        public CommandRegexAttribute(string regex)
        {
            if (string.IsNullOrWhiteSpace(regex))
            {
                throw new ArgumentException($"'{nameof(regex)}' cannot be null or whitespace.", nameof(regex));
            }

            Regex = regex;
        }

        public string Regex { get; }
    }
}
