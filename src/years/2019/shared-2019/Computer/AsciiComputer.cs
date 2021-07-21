using System;
using System.Collections.Immutable;
using Helpers;

namespace Shared2019.Computer
{
    public sealed class AsciiComputer
    {
        private static readonly int newline = '\n';
        private readonly IntcodeComputer _intcodeComputer;

        public AsciiComputer(ImmutableArray<long> memory)
        {
            _intcodeComputer = new IntcodeComputer(memory);
        }

        public void EnqueueCommand(string command)
        {
            foreach (var c in command.ToCharArray())
            {
                _intcodeComputer.Input.Enqueue((int)c);
            }

            _intcodeComputer.Input.Enqueue(newline);
        }

        public string EnqueueCommandAndRun(string command)
        {
            EnqueueCommand(command);
            return Run();
        }

        public string Run()
        {
            _ = _intcodeComputer.Run();

            var buffer = new char[_intcodeComputer.Output.Count];
            var idx = 0;
            while (_intcodeComputer.Output.TryDequeue(out var c))
            {
                buffer[idx] = (char)c;
                idx++;
            }

            var str = new string(buffer);
            return str;
        }
    }
}
