using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace MpDifficulty;

// Hades' Pact of Punishment conditions. Each rank is its own modifier type so the Custom Run
// screen can tick it; ModifierCatalog makes the ranks of one condition mutually exclusive.

public abstract class HardLabor : ModifierModel, IModifierText
{
    protected abstract int Rank { get; }

    private decimal DamageMultiplier => 1m + 0.2m * Rank;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) =>
        dealer is { IsEnemy: true } && target is { Side: CombatSide.Player } ? DamageMultiplier : 1m;

    public string TitleText(LocTable modifiers) => "Hard Labor " + MpDifficulty.Rank.Numeral(Rank);

    public string DescriptionText => $"Enemies deal [red]{20 * Rank}%[/red] more damage.";
}

public sealed class HardLabor1 : HardLabor { protected override int Rank => 1; }
public sealed class HardLabor2 : HardLabor { protected override int Rank => 2; }
public sealed class HardLabor3 : HardLabor { protected override int Rank => 3; }
public sealed class HardLabor4 : HardLabor { protected override int Rank => 4; }
public sealed class HardLabor5 : HardLabor { protected override int Rank => 5; }

/// <summary>Applied by HealPatch, since the game has no hook for healing outside Rest Sites.</summary>
public abstract class LastingConsequences : ModifierModel, IModifierText
{
    protected abstract int Rank { get; }

    public decimal HealingKept => 1m - 0.25m * Rank;

    public string TitleText(LocTable modifiers) => "Lasting Consequences " + MpDifficulty.Rank.Numeral(Rank);

    public string DescriptionText => $"All healing is reduced by [red]{25 * Rank}%[/red].";
}

public sealed class LastingConsequences1 : LastingConsequences { protected override int Rank => 1; }
public sealed class LastingConsequences2 : LastingConsequences { protected override int Rank => 2; }
public sealed class LastingConsequences3 : LastingConsequences { protected override int Rank => 3; }
public sealed class LastingConsequences4 : LastingConsequences { protected override int Rank => 4; }

public abstract class ConvenienceFee : ModifierModel, IModifierText
{
    protected abstract int Rank { get; }

    public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal cost) => cost * (1m + 0.4m * Rank);

    public string TitleText(LocTable modifiers) => "Convenience Fee " + MpDifficulty.Rank.Numeral(Rank);

    public string DescriptionText => $"[gold]Merchant[/gold] prices are [red]{40 * Rank}%[/red] higher.";
}

public sealed class ConvenienceFee1 : ConvenienceFee { protected override int Rank => 1; }
public sealed class ConvenienceFee2 : ConvenienceFee { protected override int Rank => 2; }

/// <summary>Applied by EnemyHpPatch.</summary>
public abstract class CalisthenicsProgram : ModifierModel, IModifierText
{
    protected abstract int Rank { get; }

    public decimal HpMultiplier => 1m + 0.15m * Rank;

    public string TitleText(LocTable modifiers) => "Calisthenics Program " + MpDifficulty.Rank.Numeral(Rank);

    public string DescriptionText => $"Enemies have [red]{15 * Rank}%[/red] more HP.";
}

public sealed class CalisthenicsProgram1 : CalisthenicsProgram { protected override int Rank => 1; }
public sealed class CalisthenicsProgram2 : CalisthenicsProgram { protected override int Rank => 2; }

/// <summary>
/// A flat amount of Artifact for every enemy, the same in multiplayer: ArtifactScalingPatch skips
/// the game's per-player bonus while one of these applications is in flight.
/// </summary>
public abstract class DamageControl : ModifierModel, IModifierText
{
    public static readonly HashSet<Creature> Unscaled = new();

    protected abstract int Rank { get; }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom combatRoom) return;
        foreach (Creature enemy in combatRoom.CombatState.Enemies.ToList())
        {
            await Grant(enemy);
        }
    }

    public override async Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.IsEnemy) await Grant(creature);
    }

    private async Task Grant(Creature enemy)
    {
        Unscaled.Add(enemy);
        try
        {
            await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), enemy, Rank, null, null);
        }
        finally
        {
            Unscaled.Remove(enemy);
        }
    }

    public string TitleText(LocTable modifiers) => "Damage Control " + MpDifficulty.Rank.Numeral(Rank);

    public string DescriptionText => $"All enemies start combat with [blue]{Rank}[/blue] more [gold]Artifact[/gold].";
}

public sealed class DamageControl1 : DamageControl { protected override int Rank => 1; }
public sealed class DamageControl2 : DamageControl { protected override int Rank => 2; }
public sealed class DamageControl3 : DamageControl { protected override int Rank => 3; }
public sealed class DamageControl4 : DamageControl { protected override int Rank => 4; }
