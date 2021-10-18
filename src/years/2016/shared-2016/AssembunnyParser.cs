using System.Linq;
using Shared.Parser;

namespace Shared
{
    public static class AssembunnyParser
    {
        public static CommandParser<Command> BuildParser()
        {
            var parser = new CommandParser<Command>();

            var types = typeof(Command).Assembly.GetTypes()
                .Where(t => t.BaseType == typeof(Command));

            foreach (var type in types)
            {
                parser.AddType(type);
            }

            return parser;
        }
    }
}
