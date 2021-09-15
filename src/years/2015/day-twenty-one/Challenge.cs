using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using NodaTime;
using Shared;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace DayTwentyOne2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 21), "RPG Simulator 20XX");

        public override void PartOne(IInput input, IOutput output)
        {
            var player = new Unit(UnitType.Player, 100, 0, 0, ImmutableArray<Equipment>.Empty);
            var boss = input.Parse();

            var (winner, loser) = GetPlayerVariations(player)
                .Select(variation => CalculateWinner(variation, boss))
                .Where(result => result.Winner.UnitType == UnitType.Player)
                .MinBy(result => result.Winner.TotalCost)
                .First();

            output.WriteProperty("Best Candidate", winner);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var player = new Unit(UnitType.Player, 100, 0, 0, ImmutableArray<Equipment>.Empty);
            var boss = input.Parse();

            var (winner, loser) = GetPlayerVariations(player)
                .Select(variation => CalculateWinner(variation, boss))
                .Where(result => result.Winner.UnitType == UnitType.Boss)
                .MaxBy(result => result.Loser.TotalCost)
                .First();

            output.WriteProperty("Best Candidate", loser);
        }

        private static (Unit Winner, Unit Loser) CalculateWinner(Unit player, Unit boss)
        {
            var playerTurnsForWin = boss.CalculateDeathTurn(player);
            var bossTurnsForWin = player.CalculateDeathTurn(boss);

            return playerTurnsForWin <= bossTurnsForWin
                ? (player.CalculateStateOnTurn(playerTurnsForWin, boss), boss.CalculateStateOnTurn(playerTurnsForWin, player))
                : (boss.CalculateStateOnTurn(bossTurnsForWin, player), player.CalculateStateOnTurn(bossTurnsForWin, boss));
        }

        private static IEnumerable<Unit> GetPlayerVariations(Unit player)
        {
            var equipmentSets =
                from weapon in Shop.Weapons
                from armor in Shop.Armor
                from ring1 in Shop.Rings
                from ring2 in Shop.Rings
                where ring1 != ring2
                select ImmutableArray.Create(weapon, armor, ring1, ring2);

            foreach (var equipment in equipmentSets)
            {
                yield return player.WithEquipment(equipment);
            }
        }
    }

    enum UnitType
    {
        Player,
        Boss
    }
}
