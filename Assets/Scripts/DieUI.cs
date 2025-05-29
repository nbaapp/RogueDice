using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for individual dice that can be selected for rerolling.
/// Handles click events, selection state, and visual feedback.
/// </summary>
public class DieUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Die UI References")]
    [Tooltip("The Image component that displays the die face.")]
    public Image dieImage;
    
    [Tooltip("Optional overlay image to show selection state.")]
    public Image selectionOverlay;
    
    [Tooltip("Optional text to display the die value.")]
    public TextMeshProUGUI dieValueText;
    
    [Header("Selection Visual Settings")]
    [Tooltip("Color for the selection overlay when die is selected.")]
    public Color selectedColor = new Color(1f, 1f, 0f, 0.5f); // Yellow with transparency
    
    [Tooltip("Color for the selection overlay when die is not selected.")]
    public Color deselectedColor = new Color(1f, 1f, 1f, 0f); // Transparent
    
    [Tooltip("Scale factor when die is selected (for visual feedback).")]
    public float selectedScale = 1.1f;
    
    [Tooltip("Scale factor when die is not selected.")]
    public float deselectedScale = 1.0f;
    
    [Tooltip("Duration of the selection animation.")]
    public float animationDuration = 0.2f;
    
    [Header("Roll Animation Settings")]
    [Tooltip("Duration of the dice roll animation in seconds.")]
    public float rollAnimationDuration = 1.5f;
    
    [Tooltip("How often to change the sprite during roll animation (in seconds).")]
    public float spriteChangeInterval = 0.08f;
    
    [Tooltip("How fast the die rotates during rolling (degrees per second).")]
    public float rotationSpeed = 720f;
    
    [Tooltip("Scale factor during rolling animation.")]
    public float rollingScale = 1.2f;
    
    [Tooltip("Array of all die face sprites for cycling during animation.")]
    public Sprite[] allDieFaceSprites;
    
    // Events
    [System.Serializable]
    public class DieClickEvent : UnityEngine.Events.UnityEvent<int> { }
    
    [Header("Events")]
    [Tooltip("Event fired when this die is clicked. Passes the die index.")]
    public DieClickEvent onDieClicked;
    
    // Private state
    private int dieIndex;
    private int dieValue;
    private bool isSelected = false;
    private bool isInteractable = false;
    private Vector3 originalScale;
    private System.Collections.IEnumerator animationCoroutine;
    
    // Private animation state
    private System.Collections.IEnumerator rollAnimationCoroutine;
    private bool isRolling = false;
    
    // Properties
    public bool IsSelected => isSelected;
    public bool IsInteractable => isInteractable;
    public int DieIndex => dieIndex;
    public int DieValue => dieValue;
    public bool IsRolling => isRolling;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        // Initialize selection overlay
        if (selectionOverlay != null)
        {
            selectionOverlay.color = deselectedColor;
        }
    }    /// <summary>
    /// Setup the die with its index, value, and sprite (non-animated version).
    /// </summary>
    /// <param name="index">Index of this die in the dice array.</param>
    /// <param name="value">Current value of the die.</param>
    /// <param name="sprite">Sprite to display for this die value.</param>
    public void Setup(int index, int value, Sprite sprite)
    {
        Debug.Log($"[DieUI] Setup called - setting dieIndex from {dieIndex} to {index}, value: {value}");
        
        dieIndex = index;
        dieValue = value;
        
        // Stop any ongoing animations for immediate setup
        if (rollAnimationCoroutine != null)
        {
            StopCoroutine(rollAnimationCoroutine);
            rollAnimationCoroutine = null;
            isRolling = false;
        }
        
        SetFinalState(sprite);
        
        Debug.Log($"[DieUI] Setup COMPLETED for die {index} with value {value}. DieIndex is now: {dieIndex}");
    }
    
    /// <summary>
    /// Set the selected state of this die.
    /// </summary>
    /// <param name="selected">Whether the die should be selected.</param>
    public void SetSelected(bool selected)
    {
        if (isSelected == selected) return;
        
        isSelected = selected;
        
        // Stop any ongoing animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        // Start selection animation
        animationCoroutine = AnimateSelection();
        StartCoroutine(animationCoroutine);
        
        Debug.Log($"[DieUI] Die {dieIndex} selection changed to: {selected}");
    }    /// <summary>
    /// Set whether this die can be interacted with.
    /// </summary>
    /// <param name="interactable">Whether the die should be interactable.</param>
    public void SetInteractable(bool interactable)
    {
        Debug.Log($"[DieUI] SetInteractable CALLED on die {dieIndex}: {isInteractable} -> {interactable}");
        
        // Cannot be interactable while rolling
        isInteractable = interactable && !isRolling;
        
        // Update visual state based on interactability
        if (dieImage != null)
        {
            Color imageColor = dieImage.color;
            imageColor.a = 1f; // Keep dice fully opaque regardless of interactability
            dieImage.color = imageColor;
            Debug.Log($"[DieUI] Updated die {dieIndex} visual alpha to: {imageColor.a}");
        }
        else
        {
            Debug.LogWarning($"[DieUI] Die {dieIndex} dieImage is NULL!");
        }
        
        Debug.Log($"[DieUI] Die {dieIndex} SetInteractable COMPLETED: isInteractable = {isInteractable} (requested: {interactable}, isRolling: {isRolling})");
    }    /// <summary>
    /// Handle pointer click events.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[DieUI] OnPointerClick called on die {dieIndex}. isInteractable: {isInteractable}, isSelected: {isSelected}, isRolling: {isRolling}");
        
        if (!isInteractable || isRolling)
        {
            Debug.Log($"[DieUI] Die {dieIndex} clicked but not interactable (isInteractable = {isInteractable}, isRolling = {isRolling})");
            return;
        }
        
        Debug.Log($"[DieUI] Die {dieIndex} clicked and IS interactable - processing click");
        
        // Toggle selection
        SetSelected(!isSelected);
        
        // Fire click event
        Debug.Log($"[DieUI] Die {dieIndex} firing onDieClicked event");
        onDieClicked?.Invoke(dieIndex);
    }
    
    /// <summary>
    /// Animate the selection transition.
    /// </summary>
    private System.Collections.IEnumerator AnimateSelection()
    {
        float startTime = Time.time;
        Color startColor = selectionOverlay != null ? selectionOverlay.color : Color.white;
        Vector3 startScale = transform.localScale;
        
        Color targetColor = isSelected ? selectedColor : deselectedColor;
        float targetScale = isSelected ? selectedScale : deselectedScale;
        Vector3 targetScaleVector = originalScale * targetScale;
        
        while (Time.time - startTime < animationDuration)
        {
            float progress = (Time.time - startTime) / animationDuration;
            progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth interpolation
            
            // Interpolate color
            if (selectionOverlay != null)
            {
                selectionOverlay.color = Color.Lerp(startColor, targetColor, progress);
            }
            
            // Interpolate scale
            transform.localScale = Vector3.Lerp(startScale, targetScaleVector, progress);
            
            yield return null;
        }
        
        // Ensure final values are set
        if (selectionOverlay != null)
        {
            selectionOverlay.color = targetColor;
        }
        transform.localScale = targetScaleVector;
        
        animationCoroutine = null;
    }
    
    /// <summary>
    /// Start the rolling animation for the die.
    /// </summary>
    public void StartRolling()
    {
        if (isRolling) return;
        
        Debug.Log($"[DieUI] Starting rolling animation for die {dieIndex}");
        isRolling = true;
        
        // Stop any ongoing selection animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        // Start roll animation coroutine
        rollAnimationCoroutine = AnimateRoll();
        StartCoroutine(rollAnimationCoroutine);
    }
    
    /// <summary>
    /// Animate the rolling transition.
    /// </summary>
    private System.Collections.IEnumerator AnimateRoll()
    {
        float startTime = Time.time;
        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.rotation;
        
        Vector3 targetScale = originalScale * rollingScale;
        
        // Calculate total number of sprite changes
        int totalSpriteChanges = Mathf.FloorToInt(rollAnimationDuration / spriteChangeInterval);
        int currentSpriteIndex = 0;
        
        while (Time.time - startTime < rollAnimationDuration)
        {
            float progress = (Time.time - startTime) / rollAnimationDuration;
            progress = Mathf.SmoothStep(0f, 1f, progress); // Smooth interpolation
            
            // Interpolate scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            
            // Rotate the die
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right, rotationAmount);
            transform.Rotate(Vector3.up, rotationAmount);
            transform.Rotate(Vector3.forward, rotationAmount);
            
            // Change sprite at regular intervals
            if (Time.time - startTime >= currentSpriteIndex * spriteChangeInterval)
            {
                // Update the sprite to the next in the array
                if (dieImage != null && allDieFaceSprites != null && allDieFaceSprites.Length > 0)
                {
                    currentSpriteIndex++;
                    currentSpriteIndex %= allDieFaceSprites.Length; // Loop back to 0
                    dieImage.sprite = allDieFaceSprites[currentSpriteIndex];
                }
            }
            
            yield return null;
        }
        
        // Ensure final values are set
        transform.localScale = originalScale;
        transform.rotation = startRotation;
        dieImage.sprite = allDieFaceSprites[0]; // Reset to first sprite
        
        isRolling = false;
        rollAnimationCoroutine = null;
    }
    
    /// <summary>
    /// Setup the die with animation, cycling through random sprites before showing the final result.
    /// </summary>
    /// <param name="index">Index of this die in the dice array.</param>
    /// <param name="value">Final value of the die.</param>
    /// <param name="sprite">Final sprite to display for this die value.</param>
    /// <param name="playAnimation">Whether to play the roll animation.</param>
    public void SetupWithAnimation(int index, int value, Sprite sprite, bool playAnimation = true)
    {
        Debug.Log($"[DieUI] SetupWithAnimation called - die {index}, value: {value}, playAnimation: {playAnimation}");
        
        dieIndex = index;
        dieValue = value;
        
        // Stop any existing animations
        if (rollAnimationCoroutine != null)
        {
            StopCoroutine(rollAnimationCoroutine);
            rollAnimationCoroutine = null;
        }
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        if (playAnimation && allDieFaceSprites != null && allDieFaceSprites.Length > 0)
        {
            // Start roll animation
            rollAnimationCoroutine = PlayRollAnimation(sprite);
            StartCoroutine(rollAnimationCoroutine);
        }
        else
        {
            // Set directly without animation
            SetFinalState(sprite);
        }
    }
    
    /// <summary>
    /// Play the dice rolling animation.
    /// </summary>
    /// <param name="finalSprite">The final sprite to show after animation.</param>
    private System.Collections.IEnumerator PlayRollAnimation(Sprite finalSprite)
    {
        isRolling = true;
        Debug.Log($"[DieUI] Starting roll animation for die {dieIndex}");
        
        // Store original values
        Vector3 originalRotation = transform.eulerAngles;
        Vector3 originalScale = transform.localScale;
        
        float startTime = Time.time;
        float nextSpriteChangeTime = startTime + spriteChangeInterval;
        
        // Scale up during rolling
        transform.localScale = originalScale * rollingScale;
        
        while (Time.time - startTime < rollAnimationDuration)
        {
            float elapsed = Time.time - startTime;
            
            // Rotate the die
            float rotation = (elapsed * rotationSpeed) % 360f;
            transform.eulerAngles = new Vector3(originalRotation.x, originalRotation.y, rotation);
            
            // Change sprite at intervals
            if (Time.time >= nextSpriteChangeTime)
            {
                if (allDieFaceSprites != null && allDieFaceSprites.Length > 0 && dieImage != null)
                {
                    int randomSpriteIndex = Random.Range(0, allDieFaceSprites.Length);
                    dieImage.sprite = allDieFaceSprites[randomSpriteIndex];
                }
                nextSpriteChangeTime = Time.time + spriteChangeInterval;
            }
            
            yield return null;
        }
        
        // Animation complete - set final state
        SetFinalState(finalSprite);
        
        // Restore original rotation and scale
        transform.eulerAngles = originalRotation;
        transform.localScale = originalScale;
        
        isRolling = false;
        rollAnimationCoroutine = null;
        
        Debug.Log($"[DieUI] Roll animation completed for die {dieIndex}");
    }
    
    /// <summary>
    /// Set the final state of the die after animation.
    /// </summary>
    /// <param name="finalSprite">The final sprite to display.</param>
    private void SetFinalState(Sprite finalSprite)
    {
        if (dieImage != null && finalSprite != null)
        {
            dieImage.sprite = finalSprite;
            Debug.Log($"[DieUI] Set final sprite for die {dieIndex}");
        }
        else
        {
            Debug.LogWarning($"[DieUI] Die {dieIndex} - dieImage: {(dieImage != null ? "OK" : "NULL")}, finalSprite: {(finalSprite != null ? "OK" : "NULL")}");
        }
        
        if (dieValueText != null)
        {
            dieValueText.text = dieValue.ToString();
        }
    }
}
