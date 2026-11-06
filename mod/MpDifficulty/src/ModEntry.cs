using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace MpDifficulty;

[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public static void Initialize()
    {
        new Harmony("jcorreia.MpDifficulty").PatchAll(typeof(ModEntry).Assembly);
    }
}
