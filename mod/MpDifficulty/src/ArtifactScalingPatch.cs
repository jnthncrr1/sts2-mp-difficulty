using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MpDifficulty;

[HarmonyPatch(typeof(ArtifactPower), nameof(ArtifactPower.GetScaledAmountForMultiplayer))]
public static class ArtifactScalingPatch
{
    [HarmonyPrefix]
    public static bool Before(Creature target, decimal amount, ref decimal __result)
    {
        if (!DamageControl.Unscaled.Contains(target)) return true;
        __result = amount;
        return false;
    }
}
