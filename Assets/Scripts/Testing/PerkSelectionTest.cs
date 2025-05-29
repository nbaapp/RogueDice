using UnityEngine;
using System.Linq;

/// <summary>
/// Simple test script to verify the perk selection system works correctly
/// and doesn't get stuck after round 3. This script can be attached to a GameObject
/// to run validation tests in the console.
/// </summary>
public class PerkSelectionTest : MonoBehaviour
{
    [Header("Test References")]
    [Tooltip("Reference to the PerkManager to test.")]
    public PerkManager perkManager;
    
    [Header("Test Controls")]
    [Tooltip("Run the perk selection test on Start.")]
    public bool runTestOnStart = true;
    
    void Start()
    {
        if (runTestOnStart && perkManager != null)
        {
            StartCoroutine(RunPerkSelectionTest());
        }
    }
    
    /// <summary>
    /// Test the perk selection system by simulating multiple rounds
    /// to ensure it doesn't get stuck when player owns perks.
    /// </summary>
    private System.Collections.IEnumerator RunPerkSelectionTest()
    {
        Debug.Log("[PerkSelectionTest] Starting perk selection validation test...");
        
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        if (perkManager == null)
        {
            Debug.LogError("[PerkSelectionTest] PerkManager reference is null!");
            yield break;
        }
        
        if (perkManager.allAvailablePerks == null || perkManager.allAvailablePerks.Length == 0)
        {
            Debug.LogError("[PerkSelectionTest] No available perks in PerkManager!");
            yield break;
        }
        
        // Clear perks to start fresh
        perkManager.ClearPlayerPerks();
        Debug.Log("[PerkSelectionTest] Cleared all player perks to start test");
        
        // Test multiple rounds of perk selection
        for (int round = 1; round <= 5; round++)
        {
            Debug.Log($"[PerkSelectionTest] --- Testing Round {round} ---");
            
            // Get perk choices (this is where the bug would occur)
            Perk[] choices = perkManager.GetRandomPerkChoices();
            
            if (choices == null || choices.Length == 0)
            {
                if (perkManager.GetPlayerPerkCount() < perkManager.allAvailablePerks.Length)
                {
                    Debug.LogError($"[PerkSelectionTest] FAILED: No perk choices available in round {round}, but player only has {perkManager.GetPlayerPerkCount()} perks out of {perkManager.allAvailablePerks.Length} available!");
                    yield break;
                }
                else
                {
                    Debug.Log($"[PerkSelectionTest] No more unique perks available (player owns all {perkManager.GetPlayerPerkCount()} perks) - this is expected");
                    break;
                }
            }
            
            Debug.Log($"[PerkSelectionTest] Round {round}: Got {choices.Length} valid perk choices");
            for (int i = 0; i < choices.Length; i++)
            {
                Debug.Log($"[PerkSelectionTest]   Choice {i + 1}: {choices[i].perkName} ({choices[i].GetType().Name})");
            }
            
            // Add the first choice to player's collection (simulate selection)
            if (choices.Length > 0)
            {
                bool addSuccess = perkManager.AddPerk(choices[0]);
                if (addSuccess)
                {
                    Debug.Log($"[PerkSelectionTest] Successfully added '{choices[0].perkName}' to player collection");
                    Debug.Log($"[PerkSelectionTest] Player now has {perkManager.GetPlayerPerkCount()} perks");
                }
                else
                {
                    Debug.LogError($"[PerkSelectionTest] FAILED: Could not add perk '{choices[0].perkName}' to player collection!");
                    yield break;
                }
            }
            
            // Wait a frame between tests
            yield return null;
        }
        
        Debug.Log("[PerkSelectionTest] SUCCESS: Perk selection system passed all tests!");
        Debug.Log($"[PerkSelectionTest] Final player perk count: {perkManager.GetPlayerPerkCount()}");
        
        // Display final player perks
        Perk[] finalPerks = perkManager.GetPlayerPerks();
        Debug.Log("[PerkSelectionTest] Final player perks:");
        for (int i = 0; i < finalPerks.Length; i++)
        {
            Debug.Log($"[PerkSelectionTest]   {i + 1}. {finalPerks[i].perkName} ({finalPerks[i].GetType().Name})");
        }
    }
    
    /// <summary>
    /// Manual test button accessible from inspector.
    /// </summary>
    [ContextMenu("Run Perk Selection Test")]
    public void RunManualTest()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(RunPerkSelectionTest());
        }
        else
        {
            Debug.LogWarning("[PerkSelectionTest] Test can only be run in Play mode!");
        }
    }
}
