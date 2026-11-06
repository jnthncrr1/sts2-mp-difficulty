using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Rooms;

namespace MpDifficulty;

// Each is a vanilla bad modifier with its upside removed, titled after the vanilla one.

/// <summary>Midas without the doubled gold.</summary>
public class MidasPlus : ModifierModel, IModifierText
{
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        foreach (SmithRestSiteOption smith in options.OfType<SmithRestSiteOption>().ToList())
        {
            options.Remove(smith);
        }
        return true;
    }

    public string TitleText(LocTable modifiers) => modifiers.GetRawText(ModelDb.GetEntry(typeof(Midas)) + ".title") + "+";

    public string DescriptionText => "You can no longer Smith at [gold]Rest Sites[/gold].";
}

/// <summary>Night Terrors without the full heal: resting heals normally and still costs Max HP.</summary>
public class NightTerrorsPlus : ModifierModel, IModifierText
{
    private const int MaxHpLoss = 5;

    public override async Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (!isMimicked)
        {
            int loss = Math.Min(MaxHpLoss, player.Creature.MaxHp - 1);
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), player.Creature, loss, isFromCard: false);
        }
    }

    public override IReadOnlyList<LocString> ModifyExtraRestSiteHealText(Player player, IReadOnlyList<LocString> currentExtraText)
    {
        LocString? text = AdditionalRestSiteHealText;
        if (text == null) return currentExtraText;
        text.Add("MaxHpLoss", MaxHpLoss);
        return currentExtraText.Append(text).ToList();
    }

    public string TitleText(LocTable modifiers) => modifiers.GetRawText(ModelDb.GetEntry(typeof(NightTerrors)) + ".title") + "+";

    public string DescriptionText => $"Resting at [gold]Rest Sites[/gold] costs [red]{MaxHpLoss}[/red] Max HP.";
}

/// <summary>Terminal without the Plating.</summary>
public class TerminalPlus : ModifierModel, IModifierText
{
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (RunState.BaseRoom != room) return;
        foreach (Player player in RunState.Players)
        {
            var creature = player.Creature;
            if (creature == null || creature.CurrentHp > 1 || creature.MaxHp > 1)
            {
                await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), player.Creature, 1m, isFromCard: false);
            }
        }
    }

    public string TitleText(LocTable modifiers) => modifiers.GetRawText(ModelDb.GetEntry(typeof(Terminal)) + ".title") + "+";

    public string DescriptionText => "Whenever you enter a new room, lose [red]1[/red] Max HP.";
}
