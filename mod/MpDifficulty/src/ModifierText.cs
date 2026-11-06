using MegaCrit.Sts2.Core.Localization;

namespace MpDifficulty;

/// <summary>English title and description for a mod modifier, merged into the "modifiers" table.</summary>
public interface IModifierText
{
    /// <param name="modifiers">The loaded table, for titles built from a vanilla modifier's name.</param>
    string TitleText(LocTable modifiers);

    string DescriptionText { get; }
}

public static class Rank
{
    public static string Numeral(int rank) => rank switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        4 => "IV",
        5 => "V",
        _ => rank.ToString(),
    };
}
