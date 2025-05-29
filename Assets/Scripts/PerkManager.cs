using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the perk system including available perks, selection logic, and player perk inventory.
/// </summary>
public class PerkManager : MonoBehaviour
{
    [Header("Perk Database")]
    [Tooltip("All available perks in the game.")]
    public Perk[] allAvailablePerks;

    [Header("Player Perk Settings")]
    [Tooltip("Maximum number of perks a player can hold at once.")]
    public int maxPlayerPerks = 5;

    [Tooltip("Number of perk choices offered to the player after each round.")]
    public int perkChoiceCount = 3;

    // Current player perks - this will be synchronized with GameManager
    private List<Perk> playerPerks = new List<Perk>();    /// <summary>
    /// Get a random selection of unique perks for the player to choose from.
    /// Filters out perks the player already owns.
    /// </summary>
    /// <returns>Array of unique perks for selection.</returns>
    public Perk[] GetRandomPerkChoices()
    {
        if (allAvailablePerks == null || allAvailablePerks.Length == 0)
        {
            Debug.LogWarning("[PerkManager] No available perks to choose from!");
            return new Perk[0];
        }        // Filter out null perks and perks the player already owns (by type)
        var ownedPerkTypes = playerPerks.Where(p => p != null).Select(p => p.GetType()).ToHashSet();
        var validPerks = allAvailablePerks.Where(p => p != null && !ownedPerkTypes.Contains(p.GetType())).ToList();
        
        if (validPerks.Count == 0)
        {
            Debug.LogWarning("[PerkManager] No valid perks available (player may own all perks)!");
            return new Perk[0];
        }

        // Shuffle the list and take the first N perks
        var shuffledPerks = validPerks.OrderBy(x => Random.Range(0f, 1f)).ToList();
        
        int choiceCount = Mathf.Min(perkChoiceCount, shuffledPerks.Count);
        var selectedPerks = shuffledPerks.Take(choiceCount).ToArray();

        Debug.Log($"[PerkManager] Generated {selectedPerks.Length} perk choices: [{string.Join(", ", selectedPerks.Select(p => p.perkName))}] (excluding {playerPerks.Count} owned perks)");
        
        return selectedPerks;
    }

    /// <summary>
    /// Check if the player can add more perks without exceeding the limit.
    /// </summary>
    /// <returns>True if player can add more perks, false if at limit.</returns>
    public bool CanAddMorePerks()
    {
        bool canAdd = playerPerks.Count < maxPlayerPerks;
        Debug.Log($"[PerkManager] CanAddMorePerks: {canAdd} (Current: {playerPerks.Count}/{maxPlayerPerks})");
        return canAdd;
    }

    /// <summary>
    /// Add a perk to the player's collection.
    /// </summary>
    /// <param name="perk">The perk to add.</param>
    /// <returns>True if successfully added, false if at limit.</returns>
    public bool AddPerk(Perk perk)
    {
        if (perk == null)
        {
            Debug.LogWarning("[PerkManager] Attempted to add null perk!");
            return false;
        }        if (playerPerks.Count >= maxPlayerPerks)
        {
            Debug.LogWarning($"[PerkManager] Cannot add perk '{perk.perkName}' - at maximum capacity ({maxPlayerPerks})");
            return false;
        }

        // Check if player already owns this type of perk
        if (playerPerks.Any(p => p != null && p.GetType() == perk.GetType()))
        {
            Debug.LogWarning($"[PerkManager] Cannot add perk '{perk.perkName}' - player already owns this type of perk");
            return false;
        }

        playerPerks.Add(perk);
        Debug.Log($"[PerkManager] Added perk '{perk.perkName}' to player collection. Total perks: {playerPerks.Count}");
        return true;
    }

    /// <summary>
    /// Replace a perk at the specified index with a new perk.
    /// </summary>
    /// <param name="index">Index of the perk to replace.</param>
    /// <param name="newPerk">The new perk to add.</param>
    /// <returns>True if successfully replaced, false if index is invalid.</returns>
    public bool ReplacePerk(int index, Perk newPerk)
    {
        if (newPerk == null)
        {
            Debug.LogWarning("[PerkManager] Attempted to replace with null perk!");
            return false;
        }

        if (index < 0 || index >= playerPerks.Count)
        {
            Debug.LogWarning($"[PerkManager] Invalid perk index {index} for replacement. Valid range: 0-{playerPerks.Count - 1}");
            return false;
        }

        string oldPerkName = playerPerks[index]?.perkName ?? "null";
        playerPerks[index] = newPerk;
        Debug.Log($"[PerkManager] Replaced perk '{oldPerkName}' with '{newPerk.perkName}' at index {index}");
        return true;
    }

    /// <summary>
    /// Get the current list of player perks.
    /// </summary>
    /// <returns>Array of current player perks.</returns>
    public Perk[] GetPlayerPerks()
    {
        return playerPerks.ToArray();
    }

    /// <summary>
    /// Set the player's perks (used for synchronization with GameManager).
    /// </summary>
    /// <param name="perks">Array of perks to set as player perks.</param>
    public void SetPlayerPerks(Perk[] perks)
    {
        playerPerks.Clear();
        if (perks != null)
        {
            playerPerks.AddRange(perks.Where(p => p != null));
        }
        Debug.Log($"[PerkManager] Set player perks to: [{string.Join(", ", playerPerks.Select(p => p.perkName))}]");
    }

    /// <summary>
    /// Get information about a specific player perk by index.
    /// </summary>
    /// <param name="index">Index of the perk.</param>
    /// <returns>The perk at the specified index, or null if invalid.</returns>
    public Perk GetPlayerPerkAt(int index)
    {
        if (index >= 0 && index < playerPerks.Count)
        {
            return playerPerks[index];
        }
        return null;
    }

    /// <summary>
    /// Clear all player perks (for new game).
    /// </summary>
    public void ClearPlayerPerks()
    {
        int perkCount = playerPerks.Count;
        playerPerks.Clear();
        Debug.Log($"[PerkManager] Cleared all {perkCount} player perks");
    }

    /// <summary>
    /// Get the current number of player perks.
    /// </summary>
    /// <returns>Number of perks the player currently has.</returns>
    public int GetPlayerPerkCount()
    {
        return playerPerks.Count;
    }

    /// <summary>
    /// Get the maximum number of perks a player can hold.
    /// </summary>
    /// <returns>Maximum perk capacity.</returns>
    public int GetMaxPlayerPerks()
    {
        return maxPlayerPerks;
    }
}
