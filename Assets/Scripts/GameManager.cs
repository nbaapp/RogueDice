using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // --- Configurable Properties ---
    [Header("Game Settings")]
    [Tooltip("Number of rolls the player starts with.")]
    public int startingRolls = 10;
    [Tooltip("Target score to reach in the first round.")]
    public int startingTargetScore = 15;
    [Tooltip("How much the target score increases each round.")]
    public int targetScoreIncrement = 5;
    [Tooltip("How many rolls are rewarded for completing a round.")]
    public int rollsRewardedPerRound = 3;
    [Tooltip("Number of rerolls the player starts with each round.")]
    public int startingRerolls = 3;

    // --- Game State ---
    private int currentRolls;
    private int currentTargetScore;
    private int currentScore;
    private int roundNumber;
    private bool isGameActive;
    private int currentRerolls;

    // --- Events for UI Hooks ---
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent<int, int> OnRoll; // (dice1, dice2)

    // --- Dice Reference ---
    [Header("Dice Reference")]
    [Tooltip("Reference to the Dice component used for rolling dice.")]
    public Dice dice;    // --- UI Reference ---
    [Header("UI Reference")]
    [Tooltip("Reference to the UIManager component for updating UI.")]
    public UIManager uiManager;

    // --- Perk System ---
    [Header("Perk System")]
    [Tooltip("Reference to the PerkManager component for managing perks.")]
    public PerkManager perkManager;

    [Tooltip("Reference to the PerkSelectionUI component for perk selection.")]
    public PerkSelectionUI perkSelectionUI;

    [Tooltip("List of active perks for the player. Perks are applied in order when calculating score.")]
    public Perk[] activePerks;    // Store the most recent roll results for confirmation
    private System.Collections.Generic.List<int> pendingRollResults = null;

    // Flag to track if we're waiting for perk selection
    private bool waitingForPerkSelection = false;    // Start is called before the first frame update
    void Start()
    {
        // Setup perk selection event listeners
        SetupPerkSelectionListeners();

        // Setup reroll event listeners
        if (uiManager != null)
        {
            uiManager.SetupRerollEvents(this);
        }

        // Optionally auto-start the game
        StartGame();
    }

    // --- Public Methods ---    /// <summary>
    /// Starts a new game, resets all state.
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[GameManager] Game started.");
        currentRolls = startingRolls;
        currentTargetScore = startingTargetScore;
        roundNumber = 1;
        isGameActive = true;
        waitingForPerkSelection = false;
          // Clear and reset perks
        activePerks = new Perk[0];
        if (perkManager != null)
        {
            perkManager.ClearPlayerPerks();
        }
          // Update UI to show empty perks
        if (uiManager != null)
        {
            uiManager.UpdateCurrentPerks(activePerks);
            // Ensure dice are not interactable at game start
            uiManager.SetDiceInteractable(false);
        }
        
        StartRound();
        OnGameStart?.Invoke();
    }    /// <summary>
    /// Starts a new round, resets round score.
    /// </summary>
    public void StartRound()
    {
        Debug.Log($"[GameManager] Starting round {roundNumber}. Target score: {currentTargetScore}, Rolls left: {currentRolls}");
        currentScore = 0;
        currentRerolls = startingRerolls; // Reset rerolls for the new round
        OnRoundStart?.Invoke();
        if (uiManager != null)
        {
            uiManager.OnRoundStart(roundNumber, currentTargetScore, currentRolls, currentScore);
            uiManager.UpdateRerollsLeft(currentRerolls); // Update rerolls UI
        }
    }    /// <summary>
    /// Rolls the dice using the Dice component and displays the result, but does not update score or rolls left until confirmed.
    /// </summary>
    public void RollDice()
    {
        if (!isGameActive || currentRolls <= 0 || dice == null || waitingForPerkSelection)
        {
            Debug.LogWarning("[GameManager] Cannot roll dice: Game inactive, out of rolls, Dice reference missing, or waiting for perk selection.");
            return;
        }
        if (pendingRollResults != null)
        {
            Debug.LogWarning("[GameManager] Cannot roll again: Previous roll not yet confirmed.");
            return;
        }

        // Roll the dice using the Dice component
        pendingRollResults = dice.Roll();
        int rollTotal = 0;
        foreach (var value in pendingRollResults)
            rollTotal += value;        Debug.Log($"[GameManager] Rolled dice: [{string.Join(", ", pendingRollResults)}] (Total: {rollTotal}). Awaiting confirmation.");

        // Fire OnRoll event with the first two dice (for backward compatibility)
        int dice1 = pendingRollResults.Count > 0 ? pendingRollResults[0] : 0;
        int dice2 = pendingRollResults.Count > 1 ? pendingRollResults[1] : 0;
        OnRoll?.Invoke(dice1, dice2);

        // Update UI for roll result with animation for initial rolls
        if (uiManager != null)
        {
            uiManager.UpdateRollResult(pendingRollResults, true); // Enable animation for initial rolls
        }

        // Enable reroll selection after animation completes
        StartCoroutine(EnableRerollSelectionAfterAnimation());
    }/// <summary>
    /// Handles logic when the player wins a round.
    /// </summary>
    private void WinRound()
    {
        Debug.Log($"[GameManager] Round {roundNumber} complete! Player reached target score {currentTargetScore}.");
        currentRolls += rollsRewardedPerRound;
        OnRoundEnd?.Invoke();
        
        // Show perk selection instead of immediately starting next round
        ShowPerkSelection();
    }

    /// <summary>
    /// Handles logic when the player loses the game.
    /// </summary>
    private void LoseGame()
    {
        isGameActive = false;
        Debug.Log("[GameManager] Game over! Player ran out of rolls.");
        OnGameEnd?.Invoke();
        if (uiManager != null)
        {
            uiManager.OnGameEnd();
        }
    }    /// <summary>
    /// Confirms the current roll, applies the score and updates rolls left. This is where perks/modifiers can be applied in the future.
    /// </summary>
    public void ConfirmRoll()
    {
        if (!isGameActive || pendingRollResults == null || waitingForPerkSelection)
        {
            Debug.LogWarning("[GameManager] No pending roll to confirm, game is not active, or waiting for perk selection.");
            return;
        }

        // CalculateScore now updates currentScore directly
        int calculatedScore = CalculateScore(pendingRollResults);
        currentRolls--;        Debug.Log($"[GameManager] Confirmed roll: [{string.Join(", ", pendingRollResults)}] (Score added: {calculatedScore}). Current score: {currentScore}, Rolls left: {currentRolls}");

        // Disable reroll selection since roll is confirmed
        DisableRerollSelection();

        // Update UI for score and rolls left
        if (uiManager != null)
        {
            uiManager.OnRoll(currentScore, currentRolls);
        }

        // Check for round win
        if (currentScore >= currentTargetScore)
        {
            WinRound();
            return;
        }

        // Check for game over
        if (currentRolls <= 0)
        {
            LoseGame();
        }

        // Clear pending roll after confirmation
        pendingRollResults = null;
    }

    /// <summary>
    /// Calculates the score for a given roll, applying all active perks in order. Updates the current score.
    /// </summary>
    public int CalculateScore(System.Collections.Generic.List<int> rollResults)
    {
        // Start with the base score (sum of dice)
        int score = 0;
        foreach (var value in rollResults)
            score += value;
        Debug.Log($"[GameManager] CalculateScore called. Base roll: [{string.Join(", ", rollResults)}], Base score: {score}");

        // Apply each perk in order
        if (activePerks != null)
        {
            int[] diceArray = rollResults.ToArray();
            foreach (var perk in activePerks)
            {
                if (perk != null)
                {
                    int oldScore = score;
                    score = perk.ModifyScore(score, diceArray);
                    Debug.Log($"[GameManager] Perk '{perk.perkName}' modified score: {oldScore} -> {score}");
                }
            }
        }
        currentScore += score;
        Debug.Log($"[GameManager] Final calculated score after perks: {score}, New current score: {currentScore}");
        return score;
    }

    /// <summary>
    /// Returns true if the game is currently active.
    /// </summary>
    public bool IsGameActive()
    {
        Debug.Log($"[GameManager] IsGameActive called. Result: {isGameActive}");
        return isGameActive;
    }

    /// <summary>
    /// Returns the number of rolls the player currently has.
    /// </summary>
    public int GetCurrentRolls()
    {
        Debug.Log($"[GameManager] GetCurrentRolls called. Result: {currentRolls}");
        return currentRolls;
    }

    /// <summary>
    /// Returns the current round's target score.
    /// </summary>
    public int GetCurrentTargetScore()
    {
        Debug.Log($"[GameManager] GetCurrentTargetScore called. Result: {currentTargetScore}");
        return currentTargetScore;
    }

    /// <summary>
    /// Returns the player's current score for this round.
    /// </summary>
    public int GetCurrentScore()
    {
        Debug.Log($"[GameManager] GetCurrentScore called. Result: {currentScore}");
        return currentScore;
    }

    /// <summary>
    /// Returns the current round number.
    /// </summary>
    public int GetRoundNumber()
    {
        Debug.Log($"[GameManager] GetRoundNumber called. Result: {roundNumber}");
        return roundNumber;
    }

    // --- Private Helper Methods ---

    /// <summary>
    /// Setup event listeners for perk selection UI.
    /// </summary>
    private void SetupPerkSelectionListeners()
    {
        if (perkSelectionUI != null)
        {
            perkSelectionUI.OnPerkSelected.AddListener(OnPerkSelected);
            perkSelectionUI.OnPerkReplaced.AddListener(OnPerkReplaced);
            perkSelectionUI.OnSelectionCancelled.AddListener(OnPerkSelectionCancelled);
            Debug.Log("[GameManager] Perk selection event listeners setup complete.");
        }
        else
        {
            Debug.LogWarning("[GameManager] PerkSelectionUI reference is missing!");
        }
    }    /// <summary>
    /// Handle when a perk is selected for addition (when player has space).
    /// </summary>
    private void OnPerkSelected(Perk selectedPerk)
    {
        if (perkManager != null && selectedPerk != null)
        {            if (perkManager.CanAddMorePerks())
            {
                // Player has space, add directly
                if (perkManager.AddPerk(selectedPerk))
                {
                    // Update activePerks array with new perk list
                    activePerks = perkManager.GetPlayerPerks();
                    Debug.Log($"[GameManager] Successfully added perk '{selectedPerk.perkName}' to player collection.");
                    
                    // Update UI to show current perks
                    if (uiManager != null)
                    {
                        uiManager.UpdateCurrentPerks(activePerks);
                    }
                }
                else
                {
                    Debug.LogError($"[GameManager] Failed to add perk '{selectedPerk.perkName}' to player collection!");
                }
                
                // Hide perk selection UI and continue game
                FinishPerkSelection();
            }
            else
            {
                // Player is at max capacity, show replacement UI
                if (perkSelectionUI != null)
                {
                    perkSelectionUI.ShowPerkReplacement(selectedPerk, activePerks);
                    Debug.Log($"[GameManager] Player at max capacity, showing replacement UI for '{selectedPerk.perkName}'.");
                }
                else
                {
                    Debug.LogError("[GameManager] PerkSelectionUI is null, cannot show replacement!");
                    FinishPerkSelection();
                }
            }
        }
        else
        {
            Debug.LogError("[GameManager] PerkManager or selected perk is null!");
            FinishPerkSelection();
        }
    }/// <summary>
    /// Handle when a perk is replaced (when player is at max capacity).
    /// </summary>
    private void OnPerkReplaced(int replacedIndex, Perk newPerk)
    {        if (perkManager != null && newPerk != null)
        {
            if (perkManager.ReplacePerk(replacedIndex, newPerk))
            {
                // Update activePerks array with new perk list
                activePerks = perkManager.GetPlayerPerks();
                Debug.Log($"[GameManager] Successfully replaced perk at index {replacedIndex} with '{newPerk.perkName}'.");
                
                // Update UI to show current perks
                if (uiManager != null)
                {
                    uiManager.UpdateCurrentPerks(activePerks);
                }
            }
            else
            {
                Debug.LogError($"[GameManager] Failed to replace perk at index {replacedIndex} with '{newPerk.perkName}'!");
            }
        }

        // Hide perk selection UI and continue game
        FinishPerkSelection();
    }

    /// <summary>
    /// Handle when perk selection is cancelled.
    /// </summary>
    private void OnPerkSelectionCancelled()
    {
        Debug.Log("[GameManager] Player cancelled perk selection.");
        FinishPerkSelection();
    }

    /// <summary>
    /// Finish perk selection and continue with the game.
    /// </summary>
    private void FinishPerkSelection()
    {
        waitingForPerkSelection = false;
        
        if (perkSelectionUI != null)
        {
            perkSelectionUI.HideAllPanels();
        }

        // Continue to next round
        ContinueToNextRound();
    }

    /// <summary>
    /// Continue to the next round after perk selection (or skipping).
    /// </summary>
    private void ContinueToNextRound()
    {
        roundNumber++;
        currentTargetScore += targetScoreIncrement;
        StartRound();
        pendingRollResults = null;
    }

    /// <summary>
    /// Show perk selection UI with random choices.
    /// </summary>
    private void ShowPerkSelection()
    {
        if (perkManager == null)
        {
            Debug.LogWarning("[GameManager] PerkManager is null, skipping perk selection.");
            ContinueToNextRound();
            return;
        }

        if (perkSelectionUI == null)
        {
            Debug.LogWarning("[GameManager] PerkSelectionUI is null, skipping perk selection.");
            ContinueToNextRound();
            return;
        }        // Synchronize perk manager with current active perks
        perkManager.SetPlayerPerks(activePerks);
        
        // Update UI to show current perks before selection
        if (uiManager != null)
        {
            uiManager.UpdateCurrentPerks(activePerks);
        }

        // Get random perk choices
        Perk[] choices = perkManager.GetRandomPerkChoices();
        
        if (choices.Length == 0)
        {
            Debug.LogWarning("[GameManager] No perk choices available, skipping perk selection.");
            ContinueToNextRound();
            return;
        }        waitingForPerkSelection = true;
        
        // Show appropriate UI based on whether player can add more perks
        if (perkManager.CanAddMorePerks())
        {
            perkSelectionUI.ShowPerkSelection(choices);
            Debug.Log("[GameManager] Showing perk selection - player can add more perks.");
        }
        else
        {
            // Player needs to choose which perk to replace
            // Show all choices, but user will need to pick one and then pick which to replace
            perkSelectionUI.ShowPerkSelection(choices);
            Debug.Log($"[GameManager] Showing perk selection for replacement - player at max capacity. {choices.Length} choices available.");
        }
    }

    /// <summary>
    /// Performs a reroll of selected dice using the reroll system.
    /// /// <param name="diceIndices">List of dice indices to reroll (0-based)</param>
    public void PerformReroll(System.Collections.Generic.List<int> diceIndices)
    {
        if (!isGameActive || pendingRollResults == null || dice == null || waitingForPerkSelection)
        {
            Debug.LogWarning("[GameManager] Cannot reroll: Game inactive, no pending roll, Dice reference missing, or waiting for perk selection.");
            return;
        }

        if (currentRerolls < diceIndices.Count)
        {
            Debug.LogWarning($"[GameManager] Cannot reroll: Not enough rerolls remaining. Required: {diceIndices.Count}, Available: {currentRerolls}");
            return;
        }

        if (diceIndices.Count == 0)
        {
            Debug.LogWarning("[GameManager] Cannot reroll: No dice selected.");
            return;
        }

        // Perform the reroll using the Dice component
        Debug.Log($"[GameManager] Performing reroll of dice at indices: [{string.Join(", ", diceIndices)}]");
        pendingRollResults = dice.RerollDice(diceIndices);
        currentRerolls -= diceIndices.Count;

        Debug.Log($"[GameManager] Reroll complete. New results: [{string.Join(", ", pendingRollResults)}]. Rerolls remaining: {currentRerolls}");        // Update UI with new dice results and remaining rerolls
        if (uiManager != null)
        {
            // Update roll result display (non-animated for totals)
            uiManager.UpdateRollResult(pendingRollResults, false);
            
            // Animate only the specific dice that were rerolled
            uiManager.UpdateSpecificDiceWithAnimation(pendingRollResults, diceIndices);
            
            uiManager.UpdateRerollsLeft(currentRerolls);
            
            // Use coroutine to enable interaction after animations complete
            StartCoroutine(EnableRerollSelectionAfterAnimation());
        }
    }    /// <summary>
    /// Enables dice selection for reroll after a roll is made.
    /// </summary>
    public void EnableRerollSelection()
    {
        if (uiManager != null && pendingRollResults != null && currentRerolls > 0)
        {
            Debug.Log($"[GameManager] Enabling reroll selection. Rerolls available: {currentRerolls}");
            uiManager.SetDiceInteractable(true);
            uiManager.UpdateRerollButton(false, 0);
            Debug.Log("[GameManager] Enabled dice selection for reroll.");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Cannot enable reroll selection. UIManager: {uiManager != null}, PendingRoll: {pendingRollResults != null}, Rerolls: {currentRerolls}");
        }
    }

    /// <summary>
    /// Disables dice selection for reroll.
    /// </summary>
    public void DisableRerollSelection()
    {
        if (uiManager != null)
        {
            uiManager.SetDiceInteractable(false);
            uiManager.UpdateRerollButton(false, 0);
            Debug.Log("[GameManager] Disabled dice selection for reroll.");
        }
    }

    /// <summary>
    /// Called when dice selection changes to update the reroll button.
    /// </summary>
    public void OnDiceSelectionChanged()
    {
        if (uiManager != null && pendingRollResults != null)
        {
            var selectedIndices = uiManager.GetSelectedDiceIndices();
            bool canReroll = selectedIndices.Count > 0 && selectedIndices.Count <= currentRerolls;
            uiManager.UpdateRerollButton(canReroll, selectedIndices.Count);
        }
    }

    /// <summary>
    /// Called by reroll button to perform reroll of selected dice.
    /// </summary>
    public void OnRerollButtonClicked()
    {
        if (uiManager != null)
        {
            var selectedIndices = uiManager.GetSelectedDiceIndices();
            if (selectedIndices.Count > 0)
            {
                PerformReroll(selectedIndices);
            }
        }
    }

    /// <summary>
    /// Returns the number of rerolls remaining.
    /// </summary>
    public int GetCurrentRerolls()
    {
        Debug.Log($"[GameManager] GetCurrentRerolls called. Result: {currentRerolls}");
        return currentRerolls;
    }

    /// <summary>
    /// Coroutine to enable reroll selection after dice roll animations complete.
    /// </summary>
    private System.Collections.IEnumerator EnableRerollSelectionAfterAnimation()
    {
        if (uiManager != null && pendingRollResults != null && currentRerolls > 0)
        {
            Debug.Log($"[GameManager] Waiting for dice animations to complete before enabling reroll selection. Rerolls available: {currentRerolls}");
            
            // Wait for all dice rolling animations to complete
            yield return StartCoroutine(uiManager.WaitForRollingToCompleteAndSetInteractable(true));
            
            // Update reroll button state
            uiManager.UpdateRerollButton(false, 0);
            Debug.Log("[GameManager] Enabled dice selection for reroll after animations completed.");
        }
        else
        {
            Debug.LogWarning($"[GameManager] Cannot enable reroll selection after animation. UIManager: {uiManager != null}, PendingRoll: {pendingRollResults != null}, Rerolls: {currentRerolls}");
        }
    }

    // --- Optional: Add more helper methods as needed ---
}
