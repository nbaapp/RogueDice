using UnityEngine;
using System.Collections.Generic;

public class Dice : MonoBehaviour
{
    [Header("Dice Settings")]
    [Tooltip("Number of dice to roll (e.g., 2 for 2d6)")]
    public int diceCount = 2;
    [Tooltip("Number of sides per die (e.g., 6 for d6)")]
    public int sidesPerDie = 6;

    // Stores the result of the last roll
    private List<int> lastRollResults = new List<int>();

    /// <summary>
    /// Rolls the dice and returns a list of individual die results.
    /// </summary>
    public List<int> Roll()
    {
        Debug.Log($"[Dice] Rolling {diceCount}d{sidesPerDie}...");

        // Play dice roll sound effect
        PlayDiceSound();

        lastRollResults.Clear();
        for (int i = 0; i < diceCount; i++)
        {
            // Random.Range is inclusive min, exclusive max for ints, so add 1 to sidesPerDie
            int result = Random.Range(1, sidesPerDie + 1);
            lastRollResults.Add(result);
        }
        Debug.Log($"[Dice] Roll results: [{string.Join(", ", lastRollResults)}]");
        return new List<int>(lastRollResults);
    }

    /// <summary>
    /// Returns the sum of the last roll. Returns 0 if no roll has been made yet.
    /// </summary>
    public int GetTotal()
    {
        int total = 0;
        foreach (int value in lastRollResults)
        {
            total += value;
        }
        Debug.Log($"[Dice] GetTotal called. Result: {total}");
        return total;
    }

    /// <summary>
    /// Returns a copy of the last roll results.
    /// </summary>
    public List<int> GetLastRollResults()
    {
        Debug.Log($"[Dice] GetLastRollResults called. Result: [{string.Join(", ", lastRollResults)}]");
        return new List<int>(lastRollResults);
    }

    /// <summary>
    /// Rerolls specific dice by their indices and returns the updated results.
    /// </summary>
    /// <param name="diceToReroll">List of dice indices to reroll (0-based)</param>
    /// <returns>The complete list of dice results after rerolling</returns>
    public List<int> RerollDice(List<int> diceToReroll)
    {
        if (lastRollResults.Count == 0)
        {
            Debug.LogWarning("[Dice] Cannot reroll - no previous roll results available");
            return new List<int>(lastRollResults);
        }

        Debug.Log($"[Dice] Rerolling dice at indices: [{string.Join(", ", diceToReroll)}]");

        // Play dice roll sound effect for rerolls
        PlayDiceSound();

        foreach (int index in diceToReroll)
        {
            if (index >= 0 && index < lastRollResults.Count)
            {
                int newValue = Random.Range(1, sidesPerDie + 1);
                lastRollResults[index] = newValue;
                Debug.Log($"[Dice] Rerolled die {index}: {newValue}");
            }
            else
            {
                Debug.LogWarning($"[Dice] Invalid dice index for reroll: {index}");
            }        }

        Debug.Log($"[Dice] Updated roll results: [{string.Join(", ", lastRollResults)}]");
        return new List<int>(lastRollResults);
    }    /// <summary>
    /// Plays the dice roll sound effect through the AudioManager.
    /// </summary>
    private void PlayDiceSound()
    {
        AudioManager audioManager = AudioManager.Instance;
        if (audioManager != null)
        {
            audioManager.PlayDiceRollSound();
        }
        else
        {
            Debug.LogWarning("[Dice] AudioManager not found - cannot play dice roll sound");
        }
    }
}
