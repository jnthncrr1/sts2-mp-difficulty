using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;

namespace MpDifficulty;

/// <summary>
/// Every modifier the mod adds, in Custom Run list order, and the groups of which at most one can
/// be ticked. Built on demand because ModelDb is initialized after the mod.
/// </summary>
public static class ModifierCatalog
{
    private static IReadOnlyList<ModifierModel> HardLaborRanks =>
        [ModelDb.Modifier<HardLabor1>(), ModelDb.Modifier<HardLabor2>(), ModelDb.Modifier<HardLabor3>(), ModelDb.Modifier<HardLabor4>(), ModelDb.Modifier<HardLabor5>()];

    private static IReadOnlyList<ModifierModel> LastingConsequencesRanks =>
        [ModelDb.Modifier<LastingConsequences1>(), ModelDb.Modifier<LastingConsequences2>(), ModelDb.Modifier<LastingConsequences3>(), ModelDb.Modifier<LastingConsequences4>()];

    private static IReadOnlyList<ModifierModel> ConvenienceFeeRanks =>
        [ModelDb.Modifier<ConvenienceFee1>(), ModelDb.Modifier<ConvenienceFee2>()];

    private static IReadOnlyList<ModifierModel> CalisthenicsProgramRanks =>
        [ModelDb.Modifier<CalisthenicsProgram1>(), ModelDb.Modifier<CalisthenicsProgram2>()];

    private static IReadOnlyList<ModifierModel> DamageControlRanks =>
        [ModelDb.Modifier<DamageControl1>(), ModelDb.Modifier<DamageControl2>(), ModelDb.Modifier<DamageControl3>(), ModelDb.Modifier<DamageControl4>()];

    public static IEnumerable<ModifierModel> All =>
        new ModifierModel[] { ModelDb.Modifier<MidasPlus>(), ModelDb.Modifier<NightTerrorsPlus>(), ModelDb.Modifier<TerminalPlus>() }
            .Concat(HardLaborRanks)
            .Concat(LastingConsequencesRanks)
            .Concat(ConvenienceFeeRanks)
            .Concat(CalisthenicsProgramRanks)
            .Concat(DamageControlRanks);

    public static IEnumerable<IReadOnlySet<ModifierModel>> ExclusiveGroups =>
    [
        new HashSet<ModifierModel> { ModelDb.Modifier<Midas>(), ModelDb.Modifier<MidasPlus>() },
        new HashSet<ModifierModel> { ModelDb.Modifier<NightTerrors>(), ModelDb.Modifier<NightTerrorsPlus>() },
        new HashSet<ModifierModel> { ModelDb.Modifier<Terminal>(), ModelDb.Modifier<TerminalPlus>() },
        HardLaborRanks.ToHashSet(),
        LastingConsequencesRanks.ToHashSet(),
        ConvenienceFeeRanks.ToHashSet(),
        CalisthenicsProgramRanks.ToHashSet(),
        DamageControlRanks.ToHashSet(),
    ];
}
