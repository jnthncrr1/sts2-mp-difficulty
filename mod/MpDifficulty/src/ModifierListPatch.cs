using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

namespace MpDifficulty;

/// <summary>
/// Lists the mod's modifiers on the Custom Run screen. ModelDb.BadModifiers would also work, but
/// daily runs roll their bad modifier from that list.
/// </summary>
[HarmonyPatch(typeof(NCustomRunModifiersList), "GetAllModifiers")]
public static class ModifierListPatch
{
    [HarmonyPostfix]
    public static void After(ref IEnumerable<ModifierModel> __result)
    {
        __result = __result.Concat(ModifierCatalog.All.Select(m => m.ToMutable()));
    }
}

/// <summary>
/// Ticking one modifier in a group unticks the others. Dailies only consult these groups for
/// good modifiers, which none of the mod's groups contain.
/// </summary>
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.MutuallyExclusiveModifiers), MethodType.Getter)]
public static class ExclusiveModifiersPatch
{
    [HarmonyPostfix]
    public static void After(ref IReadOnlyList<IReadOnlySet<ModifierModel>> __result)
    {
        __result = __result.Concat(ModifierCatalog.ExclusiveGroups).ToList();
    }
}
