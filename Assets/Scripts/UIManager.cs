using UnityEngine;
using TMPro; // For TextMeshProUGUI
using UnityEngine.UI; // For UI elements

public class UIManager : MonoBehaviour
{
    // --- UI Elements ---
    [Header("UI References")]
    [Tooltip("Displays the player's current score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("Displays the current target score.")]
    [SerializeField] private TextMeshProUGUI targetScoreText;

    [Tooltip("Displays the number of rolls left.")]
    [SerializeField] private TextMeshProUGUI rollsLeftText;

    [Tooltip("Displays the current round number.")]
    [SerializeField] private TextMeshProUGUI roundText;

    [Tooltip("Game Over panel or text.")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("Displays the result of the dice roll (total and individual dice).")]
    [SerializeField] private TextMeshProUGUI rollResultText;

    [Tooltip("Image(s) to display the result of each die roll.")]
    [SerializeField] private Image[] diceImages;
    [Tooltip("Sprites for each die face, index 0 = face 1, index 5 = face 6.")]
    [SerializeField] private Sprite[] dieFaceSprites;

    [Header("Reroll System")]
    [Tooltip("DieUI components for interactive dice selection.")]
    [SerializeField] private DieUI[] dieUIComponents;
    [Tooltip("Button to reroll selected dice.")]
    [SerializeField] private Button rerollButton;
    [Tooltip("Text to display remaining rerolls.")]
    [SerializeField] private TextMeshProUGUI rerollsText;    [Header("Perks Display")]
    [Tooltip("Panel or container for displaying current player perks.")]
    [SerializeField] private GameObject perksPanel;
    
    [Tooltip("Text to display current perks info.")]
    [SerializeField] private TextMeshProUGUI perksText;    [Tooltip("PerkCardUI components for displaying individual perk cards.")]
    [SerializeField] private PerkCardUI[] perkDisplayCards;

    // --- Public Methods to Update UI ---

    /// <summary>
    /// Updates the current perks display.
    /// </summary>
    public void UpdateCurrentPerks(Perk[] perks)
    {
        Debug.Log($"[UIManager] UpdateCurrentPerks called. Perks count: {(perks != null ? perks.Length : 0)}");
        
        if (perksText != null)
        {
            int perkCount = perks != null ? perks.Length : 0;
            perksText.text = $"Active Perks: {perkCount}";
        }        // Update perk cards if available
        if (perkDisplayCards != null)
        {
            for (int i = 0; i < perkDisplayCards.Length; i++)
            {
                if (perkDisplayCards[i] != null)
                {
                    if (perks != null && i < perks.Length && perks[i] != null)
                    {
                        perkDisplayCards[i].Setup(perks[i]);
                        perkDisplayCards[i].gameObject.SetActive(true);
                        
                        // Enable dragging for active perk cards (allow reordering)
                        perkDisplayCards[i].SetDraggable(true);
                    }
                    else
                    {
                        perkDisplayCards[i].gameObject.SetActive(false);
                        
                        // Disable dragging for inactive cards
                        perkDisplayCards[i].SetDraggable(false);
                    }
                }
            }
        }// Show/hide perks panel based on whether player has perks
        if (perksPanel != null)
        {
            bool hasPerks = perks != null && perks.Length > 0;
            perksPanel.SetActive(hasPerks);
        }
    }

    /// <summary>
    /// Get the array of perk display cards for external systems (like reordering).
    /// </summary>
    public PerkCardUI[] GetPerkDisplayCards()
    {
        return perkDisplayCards;
    }

    /// <summary>
    /// Updates the score UI element.
    /// </summary>
    public void UpdateScore(int score)
    {
        Debug.Log($"[UIManager] UpdateScore called. Score: {score}");
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    /// <summary>
    /// Updates the target score UI element.
    /// </summary>
    public void UpdateTargetScore(int target)
    {
        Debug.Log($"[UIManager] UpdateTargetScore called. Target: {target}");
        if (targetScoreText != null)
            targetScoreText.text = $"Target: {target}";
    }

    /// <summary>
    /// Updates the rolls left UI element.
    /// </summary>
    public void UpdateRollsLeft(int rolls)
    {
        Debug.Log($"[UIManager] UpdateRollsLeft called. Rolls left: {rolls}");
        if (rollsLeftText != null)
            rollsLeftText.text = $"Rolls Left: {rolls}";
    }

    /// <summary>
    /// Updates the round number UI element.
    /// </summary>
    public void UpdateRound(int round)
    {
        Debug.Log($"[UIManager] UpdateRound called. Round: {round}");
        if (roundText != null)
            roundText.text = $"Round: {round}";
    }

    /// <summary>
    /// Shows or hides the game over UI element.
    /// </summary>
    public void ShowGameOver(bool show)
    {
        Debug.Log($"[UIManager] ShowGameOver called. Show: {show}");
        if (gameOverPanel != null)
            gameOverPanel.SetActive(show);
    }    /// <summary>
    /// Updates the roll result UI element with the total and individual dice values.
    /// </summary>
    /// <param name="diceResults">List of individual dice results.</param>
    /// <param name="playAnimation">Whether to play roll animations for the dice.</param>
    public void UpdateRollResult(System.Collections.Generic.List<int> diceResults, bool playAnimation = true)
    {
        Debug.Log($"[UIManager] UpdateRollResult called. Dice results: {(diceResults != null ? string.Join(", ", diceResults) : "null")}, playAnimation: {playAnimation}");
        if (rollResultText != null && diceResults != null && diceResults.Count > 0)
        {
            int total = 0;
            //string diceString = "";
            for (int i = 0; i < diceResults.Count; i++)
            {
                total += diceResults[i];
                /*diceString += diceResults[i].ToString();
                if (i < diceResults.Count - 1)
                    diceString += ", ";*/
            }
            rollResultText.text = $"Total: {total}";  //(Dice: {diceString})";
        }
        else if (rollResultText != null)
        {
            rollResultText.text = "Roll: -";
        }
        // Update dice images and DieUI components
        UpdateDiceImages(diceResults);
        UpdateDiceUIWithAnimation(diceResults, playAnimation);
    }

    /// <summary>
    /// Updates the dice images to show the result of each die rolled.
    /// </summary>
    /// <param name="diceResults">List of individual dice results.</param>
    public void UpdateDiceImages(System.Collections.Generic.List<int> diceResults)
    {
        if (diceImages == null || dieFaceSprites == null)
            return;
        for (int i = 0; i < diceImages.Length; i++)
        {
            if (i < diceResults.Count && diceResults[i] > 0 && diceResults[i] <= dieFaceSprites.Length)
            {
                diceImages[i].enabled = true;
                diceImages[i].sprite = dieFaceSprites[diceResults[i] - 1];
            }
            else
            {
                diceImages[i].enabled = false; // Hide unused dice images
            }
        }    }    /// <summary>
    /// Updates the DieUI components to show the result of each die rolled.
    /// </summary>
    /// <param name="diceResults">List of individual dice results.</param>
    public void UpdateDiceUI(System.Collections.Generic.List<int> diceResults)
    {
        if (dieUIComponents == null || dieFaceSprites == null)
        {
            Debug.LogWarning("[UIManager] DieUI components or die face sprites not assigned!");
            return;
        }
            
        Debug.Log($"[UIManager] UpdateDiceUI called with {diceResults.Count} dice results");
        
        for (int i = 0; i < dieUIComponents.Length; i++)
        {
            if (dieUIComponents[i] != null)
            {
                if (i < diceResults.Count && diceResults[i] > 0 && diceResults[i] <= dieFaceSprites.Length)
                {
                    Debug.Log($"[UIManager] Setting up die {i} with value {diceResults[i]}");
                    dieUIComponents[i].Setup(i, diceResults[i], dieFaceSprites[diceResults[i] - 1]);
                    dieUIComponents[i].gameObject.SetActive(true);
                    // Ensure die is deselected initially
                    dieUIComponents[i].SetSelected(false);
                }
                else
                {
                    dieUIComponents[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning($"[UIManager] DieUI component at index {i} is null!");
            }
        }
    }

    /// <summary>
    /// Updates the DieUI components with animation to show the result of each die rolled.
    /// </summary>
    /// <param name="diceResults">List of individual dice results.</param>
    /// <param name="playAnimation">Whether to play roll animations.</param>
    public void UpdateDiceUIWithAnimation(System.Collections.Generic.List<int> diceResults, bool playAnimation = true)
    {
        if (dieUIComponents == null || dieFaceSprites == null)
        {
            Debug.LogWarning("[UIManager] DieUI components or die face sprites not assigned!");
            return;
        }
            
        Debug.Log($"[UIManager] UpdateDiceUIWithAnimation called with {diceResults.Count} dice results, playAnimation: {playAnimation}");
        
        for (int i = 0; i < dieUIComponents.Length; i++)
        {
            if (dieUIComponents[i] != null)
            {
                if (i < diceResults.Count && diceResults[i] > 0 && diceResults[i] <= dieFaceSprites.Length)
                {
                    Debug.Log($"[UIManager] Setting up die {i} with value {diceResults[i]} (animated: {playAnimation})");
                    
                    // Ensure all dice sprites are assigned to the die for animation
                    if (playAnimation && dieUIComponents[i].allDieFaceSprites == null)
                    {
                        dieUIComponents[i].allDieFaceSprites = dieFaceSprites;
                    }
                    
                    dieUIComponents[i].SetupWithAnimation(i, diceResults[i], dieFaceSprites[diceResults[i] - 1], playAnimation);
                    dieUIComponents[i].gameObject.SetActive(true);
                    // Ensure die is deselected initially
                    dieUIComponents[i].SetSelected(false);
                }
                else
                {
                    dieUIComponents[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning($"[UIManager] DieUI component at index {i} is null!");
            }
        }
    }

    /// <summary>
    /// Updates only specific dice with animation, leaving others unchanged.
    /// </summary>
    /// <param name="diceResults">List of all dice results.</param>
    /// <param name="diceIndices">Indices of dice to update with animation.</param>
    public void UpdateSpecificDiceWithAnimation(System.Collections.Generic.List<int> diceResults, System.Collections.Generic.List<int> diceIndices)
    {
        if (dieUIComponents == null || dieFaceSprites == null || diceIndices == null)
        {
            Debug.LogWarning("[UIManager] DieUI components, die face sprites, or dice indices not assigned!");
            return;
        }
            
        Debug.Log($"[UIManager] UpdateSpecificDiceWithAnimation called. Updating dice at indices: [{string.Join(", ", diceIndices)}]");
        
        foreach (int index in diceIndices)
        {
            if (index >= 0 && index < dieUIComponents.Length && dieUIComponents[index] != null)
            {
                if (index < diceResults.Count && diceResults[index] > 0 && diceResults[index] <= dieFaceSprites.Length)
                {
                    Debug.Log($"[UIManager] Animating reroll for die {index} with value {diceResults[index]}");
                    
                    // Ensure all dice sprites are assigned to the die for animation
                    if (dieUIComponents[index].allDieFaceSprites == null)
                    {
                        dieUIComponents[index].allDieFaceSprites = dieFaceSprites;
                    }
                    
                    dieUIComponents[index].SetupWithAnimation(index, diceResults[index], dieFaceSprites[diceResults[index] - 1], true);
                    dieUIComponents[index].gameObject.SetActive(true);
                    // Clear selection since this die was just rerolled
                    dieUIComponents[index].SetSelected(false);
                }
            }
            else
            {
                Debug.LogWarning($"[UIManager] Invalid dice index for reroll animation: {index}");
            }
        }
    }

    /// <summary>
    /// Updates the rerolls remaining text.
    /// </summary>
    /// <param name="rerolls">Number of rerolls remaining.</param>
    public void UpdateRerollsLeft(int rerolls)
    {
        Debug.Log($"[UIManager] UpdateRerollsLeft called. Rerolls left: {rerolls}");
        if (rerollsText != null)
            rerollsText.text = $"Rerolls: {rerolls}";
    }    /// <summary>
    /// Sets the interactable state of all dice for reroll selection.
    /// </summary>
    /// <param name="interactable">Whether dice should be interactable.</param>
    public void SetDiceInteractable(bool interactable)
    {
        Debug.Log($"[UIManager] SetDiceInteractable called with: {interactable}");
        
        if (dieUIComponents == null)
        {
            Debug.LogError("[UIManager] DieUI components array is NULL! Not assigned in Inspector!");
            return;
        }
        
        if (dieUIComponents.Length == 0)
        {
            Debug.LogError("[UIManager] DieUI components array is EMPTY! No dice assigned in Inspector!");
            return;
        }
            
        Debug.Log($"[UIManager] Setting dice interactable: {interactable}. Array length: {dieUIComponents.Length}");
        
        for (int i = 0; i < dieUIComponents.Length; i++)
        {
            var dieUI = dieUIComponents[i];
            Debug.Log($"[UIManager] Processing die {i}: dieUI={(dieUI != null ? "NOT NULL" : "NULL")}, active={(dieUI != null ? dieUI.gameObject.activeInHierarchy.ToString() : "N/A")}, rolling={(dieUI != null ? dieUI.IsRolling.ToString() : "N/A")}");
            
            if (dieUI != null && dieUI.gameObject.activeInHierarchy)
            {
                Debug.Log($"[UIManager] CALLING SetInteractable({interactable}) on die {i}");
                dieUI.SetInteractable(interactable);
                
                if (!interactable)
                {
                    dieUI.SetSelected(false); // Clear selections when disabling interaction
                }
                
                // Verify the state was actually set
                Debug.Log($"[UIManager] After SetInteractable - die {i} isInteractable: {dieUI.IsInteractable}, DieIndex: {dieUI.DieIndex}, isRolling: {dieUI.IsRolling}");
            }
            else if (dieUI == null)
            {
                Debug.LogError($"[UIManager] DieUI component at index {i} is NULL! Check Inspector assignment!");
            }
            else
            {
                Debug.LogWarning($"[UIManager] DieUI component at index {i} is INACTIVE (gameObject.activeInHierarchy = false)");
            }
        }
        
        Debug.Log($"[UIManager] SetDiceInteractable completed.");
    }

    /// <summary>
    /// Gets the indices of currently selected dice.
    /// </summary>
    /// <returns>List of selected dice indices.</returns>
    public System.Collections.Generic.List<int> GetSelectedDiceIndices()
    {
        var selectedIndices = new System.Collections.Generic.List<int>();
        
        if (dieUIComponents == null)
            return selectedIndices;
            
        foreach (var dieUI in dieUIComponents)
        {
            if (dieUI != null && dieUI.IsSelected)
            {
                selectedIndices.Add(dieUI.DieIndex);
            }
        }
        
        return selectedIndices;
    }

    /// <summary>
    /// Sets the reroll button's interactable state and updates its text.
    /// </summary>
    /// <param name="interactable">Whether the button should be interactable.</param>
    /// <param name="selectedCount">Number of dice selected for reroll.</param>
    public void UpdateRerollButton(bool interactable, int selectedCount = 0)
    {
        if (rerollButton != null)
        {
            rerollButton.interactable = interactable;
            
            // Update button text if it has a text component
            var buttonText = rerollButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                if (selectedCount > 0)
                {
                    buttonText.text = $"Reroll ({selectedCount})";
                }
                else
                {
                    buttonText.text = "Reroll";
                }
            }
        }
    }    /// <summary>
    /// Sets up event listeners for dice UI components and reroll button.
    /// Call this once during initialization.
    /// </summary>
    /// <param name="gameManager">Reference to GameManager for event handling.</param>
    public void SetupRerollEvents(GameManager gameManager)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("[UIManager] GameManager reference is null, cannot setup reroll events.");
            return;
        }

        Debug.Log("[UIManager] Setting up reroll events...");        // Setup die click events
        if (dieUIComponents != null)
        {
            Debug.Log($"[UIManager] Setting up events for {dieUIComponents.Length} dice components");
            for (int i = 0; i < dieUIComponents.Length; i++)
            {
                if (dieUIComponents[i] != null)
                {
                    dieUIComponents[i].onDieClicked.RemoveAllListeners(); // Clear existing listeners
                    // The event already passes the correct index from DieUI, so we don't need to capture it here
                    dieUIComponents[i].onDieClicked.AddListener((index) => {
                        Debug.Log($"[UIManager] Die {index} clicked, calling GameManager.OnDiceSelectionChanged()");
                        gameManager.OnDiceSelectionChanged();
                    });
                    Debug.Log($"[UIManager] Set up click event for die {i}");
                }
                else
                {
                    Debug.LogWarning($"[UIManager] DieUI component at index {i} is null!");
                }
            }
        }
        else
        {
            Debug.LogWarning("[UIManager] DieUI components array is null!");
        }

        // Setup reroll button click event
        if (rerollButton != null)
        {
            rerollButton.onClick.RemoveAllListeners(); // Clear existing listeners
            rerollButton.onClick.AddListener(() => {
                Debug.Log("[UIManager] Reroll button clicked, calling GameManager.OnRerollButtonClicked()");
                gameManager.OnRerollButtonClicked();
            });
            Debug.Log("[UIManager] Set up reroll button click event");
        }
        else
        {
            Debug.LogWarning("[UIManager] Reroll button is null!");
        }

        Debug.Log("[UIManager] Reroll event listeners setup complete.");
    }

    // --- Event Listener Methods ---
    // These can be hooked up to GameManager events or called directly.

    /// <summary>
    /// Call this when the game starts to reset UI.
    /// </summary>
    public void OnGameStart()
    {
        Debug.Log("[UIManager] OnGameStart called.");
        ShowGameOver(false);
    }

    /// <summary>
    /// Call this when a new round starts to update round UI.
    /// </summary>
    public void OnRoundStart(int round, int targetScore, int rollsLeft, int score)
    {
        Debug.Log($"[UIManager] OnRoundStart called. Round: {round}, Target: {targetScore}, Rolls: {rollsLeft}, Score: {score}");
        UpdateRound(round);
        UpdateTargetScore(targetScore);
        UpdateRollsLeft(rollsLeft);
        UpdateScore(score);
    }

    /// <summary>
    /// Call this when a roll occurs to update score and rolls left.
    /// </summary>
    public void OnRoll(int score, int rollsLeft)
    {
        Debug.Log($"[UIManager] OnRoll called. Score: {score}, Rolls left: {rollsLeft}");
        UpdateScore(score);
        UpdateRollsLeft(rollsLeft);
    }

    /// <summary>
    /// Call this when the game ends to show game over UI.
    /// </summary>
    public void OnGameEnd()
    {
        Debug.Log("[UIManager] OnGameEnd called.");
        ShowGameOver(true);
    }    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize UI components as needed
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Checks if any dice are currently playing roll animations.
    /// </summary>
    /// <returns>True if any dice are rolling, false otherwise.</returns>
    public bool AnyDiceRolling()
    {
        if (dieUIComponents == null) return false;
        
        foreach (var dieUI in dieUIComponents)
        {
            if (dieUI != null && dieUI.IsRolling)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Waits for all dice to finish rolling animations before enabling interaction.
    /// </summary>
    /// <param name="targetInteractable">The desired interactable state after rolling is complete.</param>
    /// <returns>Coroutine for waiting.</returns>
    public System.Collections.IEnumerator WaitForRollingToCompleteAndSetInteractable(bool targetInteractable)
    {
        Debug.Log($"[UIManager] Waiting for dice rolling to complete before setting interactable to: {targetInteractable}");
        
        // Wait for all dice to finish rolling
        while (AnyDiceRolling())        {
            yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
        }
        
        Debug.Log($"[UIManager] All dice finished rolling, setting interactable to: {targetInteractable}");
        SetDiceInteractable(targetInteractable);
    }
}
