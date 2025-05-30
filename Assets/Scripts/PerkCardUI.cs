using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for displaying a perk card. Handles setup of perk data, hover interactions,
/// and drag-and-drop reordering functionality.
/// </summary>
public class PerkCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("UI References")]
    [Tooltip("Text component to display the perk's name.")]
    public TextMeshProUGUI perkNameText;

    [Tooltip("Text component to display the perk's description.")]
    public TextMeshProUGUI perkDescriptionText;    [Tooltip("Image component to display the complete perk image.")]
    public Image perkImageDisplay;

    [Tooltip("Optional background image (deprecated - now using perkImage for complete card).")]
    public Image backgroundImage;

    [Header("Hover Panel")]
    [Tooltip("Description panel that appears on hover. Should be hidden by default.")]
    public GameObject descriptionPanel;    [Header("Drag Settings")]
    [Tooltip("Whether this perk card can be dragged for reordering.")]
    public bool isDraggable = false;

    // Store reference to the perk data
    private Perk currentPerk;
    
    // Drag functionality
    private bool isDragging = false;

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
            perkDescriptionText.text = perk.description;        // Set the complete perk image
        if (perkImageDisplay != null && perk.perkImage != null)
            perkImageDisplay.sprite = perk.perkImage;

        // Note: Background color changing is now deprecated since we use complete perk images
        // The backgroundImage can still be used for additional UI elements if needed
        if (backgroundImage != null)
        {
            // Set to white/transparent to not interfere with the perk image
            backgroundImage.color = Color.white;
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
    }    void Start()
    {
        // Ensure description panel is hidden on start
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    // Drag and Drop Event Handlers    /// <summary>
    /// Called when the user starts dragging this perk card.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = true;
        
        // Hide description panel during drag
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
        
        // Find and notify the reorder manager using reflection to avoid circular reference
        var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var component in allComponents)
        {
            if (component.GetType().Name == "PerkReorderManager")
            {
                // Use reflection to call StartDragging method
                var method = component.GetType().GetMethod("StartDragging");
                if (method != null)
                {
                    method.Invoke(component, new object[] { this });
                }
                break;
            }
        }
        
        Debug.Log($"[PerkCardUI] Started dragging: {currentPerk?.perkName}");
    }    /// <summary>
    /// Called continuously while the user is dragging this perk card.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || !isDragging) return;
        
        // Find and notify the reorder manager using reflection to avoid circular reference
        var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var component in allComponents)
        {
            if (component.GetType().Name == "PerkReorderManager")
            {
                // Use reflection to call UpdateDragging method
                var method = component.GetType().GetMethod("UpdateDragging");
                if (method != null)
                {
                    // Convert Vector2 to Vector3 for the method call
                    Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0f);
                    method.Invoke(component, new object[] { mousePosition });
                }
                break;
            }
        }
    }

    /// <summary>
    /// Called when the user stops dragging this perk card.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        isDragging = false;
        
        // Find and notify the reorder manager using reflection to avoid circular reference
        var allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (var component in allComponents)
        {
            if (component.GetType().Name == "PerkReorderManager")
            {
                // Use reflection to call StopDragging method
                var method = component.GetType().GetMethod("StopDragging");
                if (method != null)
                {
                    method.Invoke(component, new object[] { });
                }
                break;
            }
        }
        
        Debug.Log($"[PerkCardUI] Stopped dragging: {currentPerk?.perkName}");
    }

    /// <summary>
    /// Check if this card is currently being dragged.
    /// </summary>
    public bool IsDragging()
    {
        return isDragging;
    }

    /// <summary>
    /// Enable or disable drag functionality for this card.
    /// </summary>
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }
}
