using UnityEngine;

// Enum for perk rarity
public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

/// <summary>
/// Abstract base class for all Perks. Inherit from this to create new perk types.
/// </summary>
[CreateAssetMenu(fileName = "NewPerk", menuName = "Rogue Dice/Perk", order = 1)]
public abstract class Perk : ScriptableObject
{
    [Header("Perk Info")]
    [Tooltip("The display name of the perk.")]
    public string perkName;

    [Tooltip("A description of the perk's effect.")]
    [TextArea]
    public string description;

    [Tooltip("Icon to represent this perk in the UI.")]
    public Sprite icon;

    [Tooltip("The rarity of this perk.")]
    public Rarity rarity;

    /// <summary>
    /// Abstract method to modify the score for a roll. Implement perk logic here.
    /// </summary>
    /// <param name="rawRollScore">The unmodified score from the dice roll.</param>
    /// <param name="individualDiceResults">The array of individual dice results for this roll.</param>
    /// <returns>The modified score after applying the perk's effect.</returns>
    public abstract int ModifyScore(int rawRollScore, int[] individualDiceResults);
}
