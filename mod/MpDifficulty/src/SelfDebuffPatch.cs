using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MpDifficulty;

/// <summary>
/// Some enemies debuff themselves as part of their design, such as the Rock bowlbug's Imbalanced,
/// which makes it stun itself after a fully blocked attack. Damage Control's Artifact is granted
/// before that setup runs and would block it, so self-applied debuffs skip Artifact under Damage
/// Control. Argument 3 is the applier, which the game names "_".
/// </summary>
[HarmonyPatch(typeof(ArtifactPower), nameof(ArtifactPower.TryModifyPowerAmountReceived))]
public static class SelfDebuffPatch
{
    [HarmonyPrefix]
    public static bool Before(Creature target, decimal amount, Creature? __3, ref decimal modifiedAmount, ref bool __result)
    {
        if (__3 != target || target.CombatState?.Modifiers.OfType<DamageControl>().Any() != true) return true;
        modifiedAmount = amount;
        __result = false;
        return false;
    }
}
