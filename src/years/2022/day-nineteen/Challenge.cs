using System.Diagnostics.CodeAnalysis;

namespace DayNineteen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 19), "Not Enough Minerals");

    public void PartOne(IInput input, IOutput output)
    {
        const byte timeLimit = 24;
        var blueprints = input.Parse();

        var results = blueprints
            .Select(blueprint =>
            {
                var simulator = new Simulator(blueprint, timeLimit);
                var geodes = simulator.Simulate();

                var quality = (uint)(blueprint.Id * geodes);

                return new Result(blueprint.Id, geodes, quality);
            })
            .ToArray();

        output.WriteProperty("Total Quality", results.Sum(result => result.QualityLevel));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const byte timeLimit = 32;

        var blueprints = input.Parse();

        var results = blueprints
            .Take(3)
            .AsParallel()
            .Select(blueprint =>
            {
                var simulator = new Simulator(blueprint, timeLimit);
                var geodes = simulator.Simulate();

                var quality = (uint)(blueprint.Id * geodes);

                return new Result(blueprint.Id, geodes, quality);
            })
            .ToArray();

        var total = results.Aggregate(1, (current, result) => current * result.Geodes);

        output.WriteProperty("Largest Number of Geodes", total);
    }
}

internal readonly record struct Result(ushort Id, ushort Geodes, uint QualityLevel);

internal readonly record struct State(
    Blueprint Blueprint,
    ResourceState Robots,
    ResourceState Stockpile,
    byte Time)
{
    public ushort Geodes => Stockpile[Type.Geode];

    public ushort CalculatePotential(byte timeLimit)
    {
        var left = (byte)(timeLimit - Time);

        // If we produced a geode every minute left what would we reach?
        var potentialProduction = (left * (left + 1)) / 2;

        return (ushort)(Geodes + potentialProduction);
    }

    private IReadOnlyList<State> Next(byte timeLimit)
    {
        if (Time >= timeLimit)
        {
            return Array.Empty<State>();
        }

        var states = new List<State>(5);

        foreach (var robot in Blueprint.RobotBlueprints)
        {
            var maxNumber = Blueprint.MaxRobots[robot.RobotType];
            var current = Robots[robot.RobotType];

            var hasDependencies = Robots.IntersectsWith(robot.Resources);

            if (current < maxNumber && hasDependencies)
            {
                if (TryScheduleBuild(timeLimit, robot, out var next))
                {
                    states.Add(next.Value);
                }
            }
        }

        // If we haven't built any robots then this must be our best robot state
        // Simulate until we get to the end just gathering.
        if (states.Count is 0)
        {
            // Add a state where we never built but only gathered
            var end = timeLimit - Time;
            states.Add(this with
            {
                Time = timeLimit,
                Stockpile = Stockpile + (Robots * end),
            });
        }

        return states;
    }

    private bool TryScheduleBuild(
        int timeLimit,
        RobotBlueprint robot,
        [NotNullWhen(true)] out State? next)
    {
        var timeUntilBuild = robot.TimeUntilBuild(this);

        if (Time + timeUntilBuild > timeLimit)
        {
            next = null;
            return false;
        }

        var robots = Robots.Update(robot.RobotType, UpdateFunction);

        next = this with
        {
            Time = (byte)(Time + timeUntilBuild),
            Robots = robots,
            Stockpile = UpdateStockpile(timeUntilBuild, this, robot),
        };
        return true;

        static void UpdateFunction(Type robotType, ref ResourceState.Builder builder)
        {
            builder[robotType] += 1;
        }

        static ResourceState UpdateStockpile(
            byte time,
            State state,
            RobotBlueprint robot)
        {
            var builder = state.Stockpile.ToBuilder();

            foreach (var type in ResourceState.ResourceTypes)
            {
                var current = state.Stockpile[type];
                var removed = robot.Resources[type];
                var added = state.Robots[type] * time;

                builder[type] = (ushort)((current - removed) + added);
            }

            return builder.Build();
        }
    }

    internal IEnumerable<(State Element, ushort Priority)> CalculateNextStates(byte timeLimit)
    {
        foreach (var next in Next(timeLimit))
        {
            yield return (next, next.Geodes);
        }
    }

    public override int GetHashCode() => HashCode.Combine(
        Blueprint,
        Robots,
        Stockpile,
        Time);

    public bool Equals(State other) =>
        ReferenceEquals(Blueprint, other.Blueprint) &&
        Robots == other.Robots &&
        Stockpile == other.Stockpile &&
        Time == other.Time;
}

internal class Simulator(Blueprint blueprint, byte timeLimit)
{
    internal ushort Simulate()
    {
        var resourceState = new ResourceState(ore: 1);

        var start = new State(
            blueprint,
            resourceState,
            ResourceState.Empty,
            0);

        var frontier = new PriorityQueue<State, ushort>();
        frontier.Enqueue(start, 0);

        ushort maximumGeodes = 0;

        while (frontier.TryDequeue(out var current, out _))
        {
            var potential = current.CalculatePotential(timeLimit);

            // If the potential is better than what we've seen now then add the states
            if (potential > maximumGeodes)
            {
                frontier.EnqueueRange(current.CalculateNextStates(timeLimit));
                maximumGeodes = ushort.Max(maximumGeodes, current.Geodes);
            }
        }

        return maximumGeodes;
    }
}

record Blueprint(byte Id, ImmutableArray<RobotBlueprint> RobotBlueprints)
{
    public ResourceState MaxRobots { get; } = MaximumOfResources(RobotBlueprints);

    public override string ToString()
        => $"Blueprint {Id}: {string.Join(" ", RobotBlueprints)}";

    private static ResourceState MaximumOfResources(ImmutableArray<RobotBlueprint> robotBlueprints)
    {
        var state = new ResourceState.Builder
        {
            Geode = ushort.MaxValue
        };

        foreach (var (key, value) in robotBlueprints.SelectMany(r => r.Resources.WithValues()))
        {
            state[key] = ushort.Max(state[key], value);
        }

        return state.Build();
    }
}

record RobotBlueprint(Type RobotType, ResourceState Resources)
{
    public override string ToString()
        => $"Each {RobotType} robot costs {string.Join(" and ", Resources.WithValues()
            .Select(kvp => $"{kvp.Value} {kvp.Key}"))}.";

    public byte TimeUntilBuild(State state)
    {
        byte time = 0;

        foreach (var (type, cost) in Resources.WithValues())
        {
            var robots = state.Robots[type];
            var heldAmount = state.Stockpile[type];

            if (cost > heldAmount)
            {
                var remainder = cost - heldAmount;

                var total = remainder / (decimal)robots;

                time = byte.Max(time, (byte)decimal.Ceiling(total));
            }
        }

        // Add 1 since it's states turn
        return (byte)(time + 1);
    }
}

enum Type
{
    Ore,
    Clay,
    Obsidian,
    Geode
}
