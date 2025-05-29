using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for displaying a perk card. Handles setup of perk data and hover interactions.
/// Can also handle click events for selection.
/// </summary>
public class PerkCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    [Tooltip("Text component to display the perk's name.")]
    public TextMeshProUGUI perkNameText;

    [Tooltip("Text component to display the perk's description.")]
    public TextMeshProUGUI perkDescriptionText;

    [Tooltip("Image component to display the perk's icon.")]
    public Image perkIcon;

    [Tooltip("Optional background image that can change color based on rarity.")]
    public Image backgroundImage;

    [Header("Hover Panel")]
    [Tooltip("Description panel that appears on hover. Should be hidden by default.")]
    public GameObject descriptionPanel;

    [Header("Rarity Colors")]
    [Tooltip("Color for Common rarity perks.")]
    public Color commonColor = Color.white;

    [Tooltip("Color for Uncommon rarity perks.")]
    public Color uncommonColor = Color.green;

    [Tooltip("Color for Rare rarity perks.")]
    public Color rareColor = Color.blue;

    // Store reference to the perk data
    private Perk currentPerk;

    /// <summary>
    /// Sets up the perk card UI with data from a Perk ScriptableObject.
    /// </summary>
    /// <param name="perk">The perk data to display on this card.</param>
    public void Setup(Perk perk)
    {
        if (perk == null)
        {
            Debug.LogWarning("[PerkCardUI] Setup called with null perk.");
            return;
        }

        currentPerk = perk;

        // Set perk name
        if (perkNameText != null)
            perkNameText.text = perk.perkName;

        // Set perk description
        if (perkDescriptionText != null)
            perkDescriptionText.text = perk.description;

        // Set perk icon
        if (perkIcon != null && perk.icon != null)
            perkIcon.sprite = perk.icon;

        // Set background color based on rarity
        if (backgroundImage != null)
        {
            switch (perk.rarity)
            {
                case Rarity.Common:
                    backgroundImage.color = commonColor;
                    break;
                case Rarity.Uncommon:
                    backgroundImage.color = uncommonColor;
                    break;
                case Rarity.Rare:
                    backgroundImage.color = rareColor;
                    break;
            }
        }

        // Ensure description panel is hidden by default
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);

        Debug.Log($"[PerkCardUI] Setup complete for perk: {perk.perkName}");
    }

    /// <summary>
    /// Called when the mouse pointer enters the card area. Shows the description panel.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(true);
            Debug.Log($"[PerkCardUI] Showing description panel for: {currentPerk?.perkName}");
        }
    }    /// <summary>
    /// Called when the mouse pointer exits the card area. Hides the description panel.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(false);
            Debug.Log($"[PerkCardUI] Hiding description panel for: {currentPerk?.perkName}");
        }
    }

    /// <summary>
    /// Called when the card is clicked. Can be used for selection functionality.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentPerk != null)
        {
            Debug.Log($"[PerkCardUI] Card clicked for perk: {currentPerk.perkName}");
            // This can be extended later if direct click handling is needed
        }
    }

    /// <summary>
    /// Returns the currently assigned perk data.
    /// </summary>
    public Perk GetCurrentPerk()
    {
        return currentPerk;
    }

    void Start()
    {
        // Ensure description panel is hidden on start
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }
}
