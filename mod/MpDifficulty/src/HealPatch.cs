using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace MpDifficulty;

/// <summary>
/// Every heal goes through CreatureCmd.Heal. Only players are affected, and a heal on a dead
/// player is left alone: that is the multiplayer revive to 1 HP at the end of combat, and
/// reducing it would leave the player dead into the next fight.
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Heal))]
public static class HealPatch
{
    [HarmonyPrefix]
    public static void Before(Creature creature, ref decimal amount)
    {
        if (creature.Player == null || creature.IsDead) return;
        LastingConsequences? consequences = creature.Player.RunState.Modifiers.OfType<LastingConsequences>().FirstOrDefault();
        if (consequences == null) return;
        amount = Math.Floor(amount * consequences.HealingKept);
    }
}
