namespace DayTwentyFive2017;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 25), "The Halting Problem");

    public void PartOne(IInput input, IOutput output)
    {
        var (machine, iterations) = input.ParseMachine();

        var result = machine.Run(iterations);

        output.WriteProperty("Diagnostic Checksum", result.Tape.Count);
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex beginStateRegex = new("Begin in state ([A-Z]).");
    private static readonly PcreRegex iterationsRegex = new(@"Perform a diagnostic checksum after (\d+) steps");

    public static (StateMachine Machine, int Iterations) ParseMachine(this IInput input)
    {
        var paragraphs = input.Lines.AsParagraphs();

        var (start, iterations) = ParseInitialInfo(paragraphs[0].Span);
        var states = ParseStates(paragraphs.AsSpan()[1..]);

        var machine = new StateMachine(
            states,
            start,
            0,
            ImmutableHashSet<int>.Empty);

        return (machine, iterations);
    }

    private static (char StateState, int Iterations) ParseInitialInfo(ReadOnlySpan<ReadOnlyMemory<char>> lines)
    {
        var state = beginStateRegex.Match(lines[0].Span)[1].Value[0];
        var iterations = int.Parse(iterationsRegex.Match(lines[1].Span)[1].Value);

        return (state, iterations);
    }

    private static ImmutableDictionary<char, State> ParseStates(ReadOnlySpan<ReadOnlyMemory<ReadOnlyMemory<char>>> paragraphs)
    {
        var builder = ImmutableDictionary.CreateBuilder<char, State>();

        foreach (var paragraph in paragraphs)
        {
            var state = ParseState(paragraph.Span);
            builder.Add(state.Name, state);
        }

        return builder.ToImmutable();

        static State ParseState(ReadOnlySpan<ReadOnlyMemory<char>> span)
        {
            var state = span[0].Span[^2];
            var zero = ParseMachineStep(span[2..5]);
            var one = ParseMachineStep(span[6..]);

            return new State(state, zero, one);
        }

        static Func<StateMachine, StateMachine> ParseMachineStep(ReadOnlySpan<ReadOnlyMemory<char>> lines)
        {
            var writeValue = lines[0].Span[^2] == '1';
            var slotOffset = lines[1].Span[..^1].EndsWith("left") ? -1 : +1;
            var nextState = lines[2].Span[^2];

            return state =>
            {
                var tape = writeValue
                    ? state.Tape.Add(state.Cursor)
                    : state.Tape.Remove(state.Cursor);

                return state with
                {
                    Cursor = state.Cursor + slotOffset,
                    CurrentState = nextState,
                    Tape = tape
                };
            };
        }
    }
}

record StateMachine(ImmutableDictionary<char, State> States, char CurrentState, int Cursor, ImmutableHashSet<int> Tape)
{
    internal StateMachine Run(int iterations)
    {
        var current = this;

        for (var i = 0; i < iterations; i++)
        {
            var state = current.States[current.CurrentState];
            current = current.Tape.Contains(current.Cursor)
                ? state.OneFunc(current)
                : state.ZeroFunc(current);
        }

        return current;
    }
}

record State(char Name, Func<StateMachine, StateMachine> ZeroFunc, Func<StateMachine, StateMachine> OneFunc);
