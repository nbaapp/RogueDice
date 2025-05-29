using UnityEngine;

/// <summary>
/// Simple testing script to validate the perk selection system.
/// Attach this to a GameObject in your scene for debugging.
/// </summary>
public class PerkSystemTester : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the GameManager to test.")]
    public GameManager gameManager;

    [Tooltip("Reference to the PerkManager to test.")]
    public PerkManager perkManager;

    [Header("Test Controls")]
    [Tooltip("Test adding a specific perk (requires perk assets to be assigned in PerkManager).")]
    public bool testAddPerk = false;

    [Tooltip("Test showing perk selection UI.")]
    public bool testShowPerkSelection = false;

    [Tooltip("Test clearing all perks.")]
    public bool testClearPerks = false;

    private void Update()
    {
        // Test controls via inspector checkboxes
        if (testAddPerk)
        {
            testAddPerk = false;
            TestAddRandomPerk();
        }

        if (testShowPerkSelection)
        {
            testShowPerkSelection = false;
            TestShowPerkSelection();
        }

        if (testClearPerks)
        {
            testClearPerks = false;
            TestClearPerks();
        }

        // Keyboard shortcuts for testing
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestShowPerkSelection();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TestClearPerks();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            TestAddRandomPerk();
        }
    }

    private void TestAddRandomPerk()
    {
        if (perkManager == null)
        {
            Debug.LogWarning("[PerkSystemTester] PerkManager reference is null!");
            return;
        }

        Perk[] choices = perkManager.GetRandomPerkChoices();
        if (choices.Length > 0)
        {
            bool success = perkManager.AddPerk(choices[0]);
            Debug.Log($"[PerkSystemTester] Attempted to add perk '{choices[0].perkName}': {(success ? "Success" : "Failed")}");
            
            // Update GameManager active perks
            if (gameManager != null)
            {
                gameManager.activePerks = perkManager.GetPlayerPerks();
            }
        }
        else
        {
            Debug.LogWarning("[PerkSystemTester] No perk choices available!");
        }
    }

    private void TestShowPerkSelection()
    {
        if (gameManager == null)
        {
            Debug.LogWarning("[PerkSystemTester] GameManager reference is null!");
            return;
        }

        // Use reflection to call the private ShowPerkSelection method
        var method = typeof(GameManager).GetMethod("ShowPerkSelection", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            method.Invoke(gameManager, null);
            Debug.Log("[PerkSystemTester] Called ShowPerkSelection method");
        }
        else
        {
            Debug.LogWarning("[PerkSystemTester] Could not find ShowPerkSelection method!");
        }
    }

    private void TestClearPerks()
    {
        if (perkManager == null)
        {
            Debug.LogWarning("[PerkSystemTester] PerkManager reference is null!");
            return;
        }

        int perkCount = perkManager.GetPlayerPerkCount();
        perkManager.ClearPlayerPerks();
        Debug.Log($"[PerkSystemTester] Cleared {perkCount} perks");

        // Update GameManager active perks
        if (gameManager != null)
        {
            gameManager.activePerks = perkManager.GetPlayerPerks();
        }
    }

    private void OnGUI()
    {
        // Simple debug GUI for testing
        if (perkManager != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Player Perks: {perkManager.GetPlayerPerkCount()}/{perkManager.GetMaxPlayerPerks()}");
            
            Perk[] playerPerks = perkManager.GetPlayerPerks();
            for (int i = 0; i < playerPerks.Length; i++)
            {
                if (playerPerks[i] != null)
                {
                    GUILayout.Label($"  {i + 1}. {playerPerks[i].perkName} ({playerPerks[i].rarity})");
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Test Controls:");
            GUILayout.Label("P - Show Perk Selection");
            GUILayout.Label("A - Add Random Perk");
            GUILayout.Label("C - Clear All Perks");
            GUILayout.EndArea();
        }
    }
}
