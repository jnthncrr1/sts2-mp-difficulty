using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace MpDifficulty;

/// <summary>
/// Enemy HP is rolled (and scaled for the player count in multiplayer) inside CreateCreature,
/// before the creature is attached to the combat, so the extra scaling goes on its result.
/// </summary>
[HarmonyPatch(typeof(CombatState), nameof(CombatState.CreateCreature))]
public static class EnemyHpPatch
{
    [HarmonyPostfix]
    public static void After(CombatState __instance, Creature __result)
    {
        if (!__result.IsEnemy) return;
        CalisthenicsProgram? calisthenics = __instance.RunState.Modifiers.OfType<CalisthenicsProgram>().FirstOrDefault();
        if (calisthenics == null) return;
        __result.SetMaxHpInternal(Math.Round(__result.MaxHp * calisthenics.HpMultiplier));
        __result.SetCurrentHpInternal(__result.MaxHp);
        GD.Print($"[MpDifficulty] {__result.Name} HP scaled to {__result.MaxHp}");
    }
}
