using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Test script to validate the reroll system functionality.
/// Attach this to a GameObject in your scene for debugging the reroll feature.
/// </summary>
public class RerollSystemTest : MonoBehaviour
{
    [Header("Test References")]
    [Tooltip("Reference to the GameManager to test.")]
    public GameManager gameManager;
    
    [Tooltip("Reference to the UIManager to test.")]
    public UIManager uiManager;
    
    [Tooltip("Reference to the Dice component to test.")]
    public Dice dice;
    
    [Header("Test Controls")]
    [Tooltip("Run dice selection tests on Start.")]
    public bool runTestOnStart = true;
    
    [Tooltip("Test rolling dice and enabling reroll selection.")]
    public bool testRollAndReroll = false;
    
    [Tooltip("Test individual die selection.")]
    public bool testDieSelection = false;
    
    [Tooltip("Test reroll button functionality.")]
    public bool testRerollButton = false;

    void Start()
    {
        if (runTestOnStart)
        {
            StartCoroutine(RunRerollTests());
        }
    }

    void Update()
    {
        // Test controls via inspector checkboxes
        if (testRollAndReroll)
        {
            testRollAndReroll = false;
            TestRollAndReroll();
        }
        
        if (testDieSelection)
        {
            testDieSelection = false;
            TestDieSelection();
        }
        
        if (testRerollButton)
        {
            testRerollButton = false;
            TestRerollButton();
        }
        
        // Keyboard shortcuts for testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            TestRollAndReroll();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            TestDieSelection();
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            TestRerollButton();
        }
    }

    /// <summary>
    /// Run comprehensive reroll system tests.
    /// </summary>
    private System.Collections.IEnumerator RunRerollTests()
    {
        Debug.Log("[RerollSystemTest] Starting reroll system validation tests...");
        
        // Wait a frame to ensure everything is initialized
        yield return null;
        
        if (!ValidateReferences())
        {
            yield break;
        }
        
        // Test 1: Roll dice and check if reroll selection is enabled
        Debug.Log("[RerollSystemTest] Test 1: Rolling dice and checking reroll selection...");
        TestRollAndReroll();
        
        yield return new UnityEngine.WaitForSeconds(1f);
        
        // Test 2: Check die selection functionality
        Debug.Log("[RerollSystemTest] Test 2: Testing die selection...");
        TestDieSelection();
        
        yield return new UnityEngine.WaitForSeconds(1f);
        
        // Test 3: Test reroll button
        Debug.Log("[RerollSystemTest] Test 3: Testing reroll button...");
        TestRerollButton();
        
        Debug.Log("[RerollSystemTest] Reroll system tests completed.");
    }

    /// <summary>
    /// Validate that all required references are assigned.
    /// </summary>
    private bool ValidateReferences()
    {
        bool valid = true;
        
        if (gameManager == null)
        {
            Debug.LogError("[RerollSystemTest] GameManager reference is null!");
            valid = false;
        }
        
        if (uiManager == null)
        {
            Debug.LogError("[RerollSystemTest] UIManager reference is null!");
            valid = false;
        }
        
        if (dice == null)
        {
            Debug.LogError("[RerollSystemTest] Dice reference is null!");
            valid = false;
        }
        
        return valid;
    }

    /// <summary>
    /// Test rolling dice and enabling reroll selection.
    /// </summary>
    private void TestRollAndReroll()
    {
        if (!ValidateReferences()) return;
        
        Debug.Log("[RerollSystemTest] Testing roll and reroll enablement...");
        
        // Check initial state
        Debug.Log($"[RerollSystemTest] Initial rerolls: {gameManager.GetCurrentRerolls()}");
        Debug.Log($"[RerollSystemTest] Game active: {gameManager.IsGameActive()}");
        
        // Roll dice
        gameManager.RollDice();
        
        // Check if reroll selection was enabled
        Debug.Log($"[RerollSystemTest] After roll - rerolls available: {gameManager.GetCurrentRerolls()}");
        
        // Check dice UI state
        if (uiManager != null)
        {
            var selectedIndices = uiManager.GetSelectedDiceIndices();
            Debug.Log($"[RerollSystemTest] Currently selected dice: {selectedIndices.Count}");
        }
    }

    /// <summary>
    /// Test individual die selection functionality.
    /// </summary>
    private void TestDieSelection()
    {
        if (!ValidateReferences()) return;
        
        Debug.Log("[RerollSystemTest] Testing die selection...");
        
        // Get current selected dice
        if (uiManager != null)
        {
            var selectedIndices = uiManager.GetSelectedDiceIndices();
            Debug.Log($"[RerollSystemTest] Currently selected dice: [{string.Join(", ", selectedIndices)}]");
            
            // Test dice interactability state by checking if dice are properly set up
            Debug.Log("[RerollSystemTest] Checking dice UI setup...");
            
            // Simulate dice selection changes
            gameManager.OnDiceSelectionChanged();
            
            // Check reroll button state after selection change
            Debug.Log("[RerollSystemTest] Die selection test completed.");
        }
    }

    /// <summary>
    /// Test reroll button functionality.
    /// </summary>
    private void TestRerollButton()
    {
        if (!ValidateReferences()) return;
        
        Debug.Log("[RerollSystemTest] Testing reroll button...");
        
        // Check current state
        int rerollsBefore = gameManager.GetCurrentRerolls();
        Debug.Log($"[RerollSystemTest] Rerolls before button test: {rerollsBefore}");
        
        // Get selected dice
        if (uiManager != null)
        {
            var selectedIndices = uiManager.GetSelectedDiceIndices();
            Debug.Log($"[RerollSystemTest] Selected dice for reroll: [{string.Join(", ", selectedIndices)}]");
            
            if (selectedIndices.Count > 0 && selectedIndices.Count <= rerollsBefore)
            {
                // Simulate reroll button click
                gameManager.OnRerollButtonClicked();
                
                int rerollsAfter = gameManager.GetCurrentRerolls();
                Debug.Log($"[RerollSystemTest] Rerolls after button click: {rerollsAfter}");
                Debug.Log($"[RerollSystemTest] Rerolls consumed: {rerollsBefore - rerollsAfter}");
            }
            else
            {
                Debug.Log("[RerollSystemTest] Cannot test reroll button - no valid dice selection or insufficient rerolls.");
            }
        }
    }

    /// <summary>
    /// Manual test methods accessible from inspector or context menu.
    /// </summary>
    [ContextMenu("Test Roll and Reroll")]
    public void ManualTestRollAndReroll()
    {
        TestRollAndReroll();
    }
    
    [ContextMenu("Test Die Selection")]
    public void ManualTestDieSelection()
    {
        TestDieSelection();
    }
    
    [ContextMenu("Test Reroll Button")]
    public void ManualTestRerollButton()
    {
        TestRerollButton();
    }
    
    [ContextMenu("Run All Tests")]
    public void ManualRunAllTests()
    {
        StartCoroutine(RunRerollTests());
    }
}
