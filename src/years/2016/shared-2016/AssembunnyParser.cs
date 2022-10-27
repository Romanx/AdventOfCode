using Shared.Parser;

namespace Shared
{
    public static class AssembunnyParser
    {
        public static CommandParser<Command> BuildParser()
        {
            var parser = new CommandParser<Command>()
                .AddDerivedTypes<Command>();

            return parser;
        }
    }
}
