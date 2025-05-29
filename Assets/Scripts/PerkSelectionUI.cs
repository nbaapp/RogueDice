using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// UI manager for perk selection after round completion. Handles both new perk selection and perk replacement.
/// </summary>
public class PerkSelectionUI : MonoBehaviour
{
    [Header("Selection Panel")]
    [Tooltip("Main panel containing the perk selection UI.")]
    public GameObject selectionPanel;

    [Tooltip("Title text for the selection panel.")]
    public TextMeshProUGUI selectionTitle;

    [Header("Perk Choice Cards")]
    [Tooltip("Array of PerkCardUI components for displaying perk choices.")]
    public PerkCardUI[] choiceCards;

    [Tooltip("Array of buttons corresponding to each choice card.")]
    public Button[] choiceButtons;

    [Header("Replacement Panel")]
    [Tooltip("Panel for selecting which perk to replace when at max capacity.")]
    public GameObject replacementPanel;

    [Tooltip("Title text for the replacement panel.")]
    public TextMeshProUGUI replacementTitle;

    [Tooltip("Array of PerkCardUI components for displaying current player perks.")]
    public PerkCardUI[] playerPerkCards;

    [Tooltip("Array of buttons for selecting which perk to replace.")]
    public Button[] replacementButtons;

    [Header("Cancel Button")]
    [Tooltip("Button to cancel perk selection (skip the reward).")]
    public Button cancelButton;

    // Events
    public UnityEvent<Perk> OnPerkSelected;
    public UnityEvent<int, Perk> OnPerkReplaced; // (index, newPerk)
    public UnityEvent OnSelectionCancelled;

    // Internal state
    private Perk[] currentChoices;
    private Perk selectedPerkForReplacement;
    private bool isInReplacementMode = false;

    private void Awake()
    {
        // Setup button listeners
        SetupChoiceButtons();
        SetupReplacementButtons();
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelSelection);
        }

        // Hide panels by default
        HideAllPanels();
    }

    /// <summary>
    /// Show the perk selection UI with the given perk choices.
    /// </summary>
    /// <param name="perkChoices">Array of perks to choose from.</param>
    public void ShowPerkSelection(Perk[] perkChoices)
    {
        if (perkChoices == null || perkChoices.Length == 0)
        {
            Debug.LogWarning("[PerkSelectionUI] ShowPerkSelection called with null or empty choices!");
            return;
        }

        currentChoices = perkChoices;
        isInReplacementMode = false;

        // Setup choice cards
        SetupChoiceCards(perkChoices);

        // Show selection panel
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
        }

        if (selectionTitle != null)
        {
            selectionTitle.text = "Choose a Perk Reward!";
        }

        Debug.Log($"[PerkSelectionUI] Showing perk selection with {perkChoices.Length} choices");
    }

    /// <summary>
    /// Show the perk replacement UI when player is at max capacity.
    /// </summary>
    /// <param name="newPerk">The new perk to potentially add.</param>
    /// <param name="currentPerks">Array of current player perks.</param>
    public void ShowPerkReplacement(Perk newPerk, Perk[] currentPerks)
    {
        if (newPerk == null || currentPerks == null)
        {
            Debug.LogWarning("[PerkSelectionUI] ShowPerkReplacement called with null parameters!");
            return;
        }

        selectedPerkForReplacement = newPerk;
        isInReplacementMode = true;

        // Setup current perk cards
        SetupPlayerPerkCards(currentPerks);

        // Hide selection panel and show replacement panel
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        if (replacementPanel != null)
        {
            replacementPanel.SetActive(true);
        }

        if (replacementTitle != null)
        {
            replacementTitle.text = $"Replace a perk with '{newPerk.perkName}'?";
        }

        Debug.Log($"[PerkSelectionUI] Showing perk replacement for '{newPerk.perkName}' with {currentPerks.Length} current perks");
    }

    /// <summary>
    /// Hide all UI panels.
    /// </summary>
    public void HideAllPanels()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        if (replacementPanel != null)
        {
            replacementPanel.SetActive(false);
        }

        isInReplacementMode = false;
        selectedPerkForReplacement = null;
        currentChoices = null;

        Debug.Log("[PerkSelectionUI] Hidden all panels");
    }

    /// <summary>
    /// Setup the choice cards with perk data.
    /// </summary>
    private void SetupChoiceCards(Perk[] choices)
    {
        for (int i = 0; i < choiceCards.Length; i++)
        {
            if (choiceCards[i] != null)
            {
                if (i < choices.Length && choices[i] != null)
                {
                    choiceCards[i].Setup(choices[i]);
                    choiceCards[i].gameObject.SetActive(true);
                }
                else
                {
                    choiceCards[i].gameObject.SetActive(false);
                }
            }

            // Enable/disable corresponding buttons
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].gameObject.SetActive(i < choices.Length && choices[i] != null);
            }
        }
    }

    /// <summary>
    /// Setup the player perk cards for replacement selection.
    /// </summary>
    private void SetupPlayerPerkCards(Perk[] playerPerks)
    {
        for (int i = 0; i < playerPerkCards.Length; i++)
        {
            if (playerPerkCards[i] != null)
            {
                if (i < playerPerks.Length && playerPerks[i] != null)
                {
                    playerPerkCards[i].Setup(playerPerks[i]);
                    playerPerkCards[i].gameObject.SetActive(true);
                }
                else
                {
                    playerPerkCards[i].gameObject.SetActive(false);
                }
            }

            // Enable/disable corresponding buttons
            if (replacementButtons[i] != null)
            {
                replacementButtons[i].gameObject.SetActive(i < playerPerks.Length && playerPerks[i] != null);
            }
        }
    }

    /// <summary>
    /// Setup button listeners for choice buttons.
    /// </summary>
    private void SetupChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
            {
                int choiceIndex = i; // Capture for closure
                choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClicked(choiceIndex));
            }
        }
    }

    /// <summary>
    /// Setup button listeners for replacement buttons.
    /// </summary>
    private void SetupReplacementButtons()
    {
        for (int i = 0; i < replacementButtons.Length; i++)
        {
            if (replacementButtons[i] != null)
            {
                int perkIndex = i; // Capture for closure
                replacementButtons[i].onClick.AddListener(() => OnReplacementButtonClicked(perkIndex));
            }
        }
    }

    /// <summary>
    /// Handle choice button clicks.
    /// </summary>
    private void OnChoiceButtonClicked(int choiceIndex)
    {
        if (currentChoices == null || choiceIndex >= currentChoices.Length)
        {
            Debug.LogWarning($"[PerkSelectionUI] Invalid choice index: {choiceIndex}");
            return;
        }

        Perk selectedPerk = currentChoices[choiceIndex];
        if (selectedPerk == null)
        {
            Debug.LogWarning($"[PerkSelectionUI] Selected perk at index {choiceIndex} is null!");
            return;
        }

        Debug.Log($"[PerkSelectionUI] Choice button clicked: {selectedPerk.perkName}");
        OnPerkSelected?.Invoke(selectedPerk);
    }

    /// <summary>
    /// Handle replacement button clicks.
    /// </summary>
    private void OnReplacementButtonClicked(int perkIndex)
    {
        if (selectedPerkForReplacement == null)
        {
            Debug.LogWarning("[PerkSelectionUI] No perk selected for replacement!");
            return;
        }

        Debug.Log($"[PerkSelectionUI] Replacement button clicked: replacing perk at index {perkIndex} with {selectedPerkForReplacement.perkName}");
        OnPerkReplaced?.Invoke(perkIndex, selectedPerkForReplacement);
    }

    /// <summary>
    /// Handle cancel button click.
    /// </summary>
    private void CancelSelection()
    {
        Debug.Log("[PerkSelectionUI] Selection cancelled by player");
        OnSelectionCancelled?.Invoke();
    }

    /// <summary>
    /// Check if the UI is currently active.
    /// </summary>
    public bool IsActive()
    {
        bool selectionActive = selectionPanel != null && selectionPanel.activeInHierarchy;
        bool replacementActive = replacementPanel != null && replacementPanel.activeInHierarchy;
        return selectionActive || replacementActive;
    }

    /// <summary>
    /// Check if currently in replacement mode.
    /// </summary>
    public bool IsInReplacementMode()
    {
        return isInReplacementMode;
    }
}
