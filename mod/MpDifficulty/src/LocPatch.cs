using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace MpDifficulty;

/// <summary>
/// The game only reads modded text from a mod's .pck, so the strings are merged into the loaded
/// "modifiers" table instead. English for every language.
/// </summary>
public static class ModifierTextMerger
{
    public static void Merge(LocTable modifiers)
    {
        var strings = new Dictionary<string, string>();
        foreach (ModifierModel modifier in ModifierCatalog.All)
        {
            var text = (IModifierText)modifier;
            strings[modifier.Id.Entry + ".title"] = text.TitleText(modifiers);
            strings[modifier.Id.Entry + ".description"] = text.DescriptionText;
        }
        string nightTerrorsPlus = ModelDb.GetEntry(typeof(NightTerrorsPlus));
        strings[nightTerrorsPlus + ".additionalRestSiteHealText"] = "Lose [red]{MaxHpLoss}[/red] Max HP.";
        strings[nightTerrorsPlus + ".additionalRestSiteMendText"] = "They lose [red]{MaxHpLoss}[/red] Max HP.";
        modifiers.MergeWith(strings);
    }

    public static void TryMerge(Func<LocTable?> modifiers)
    {
        try
        {
            LocTable? table = modifiers();
            if (table != null) Merge(table);
        }
        catch (Exception e)
        {
            GD.PushWarning("[MpDifficulty] modifier text merge failed: " + e);
        }
    }
}

/// <summary>
/// The text tables load before ModelDb is initialized, so the first merge waits for ModelDb.
/// </summary>
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Init))]
public static class ModelDbInitPatch
{
    [HarmonyPostfix]
    public static void After() => ModifierTextMerger.TryMerge(() => LocManager.Instance?.GetTable("modifiers"));
}

/// <summary>Re-merges after a language change replaces the tables.</summary>
[HarmonyPatch(typeof(LocManager), "SetLanguageInternal")]
public static class LocPatch
{
    [HarmonyPostfix]
    public static void After(Dictionary<string, LocTable> tables)
    {
        if (!ModelDb.Contains(typeof(MidasPlus))) return;
        ModifierTextMerger.TryMerge(() => tables.GetValueOrDefault("modifiers"));
    }
}
