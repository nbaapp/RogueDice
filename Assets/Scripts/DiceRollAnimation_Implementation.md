# Dice Roll Animation Implementation

## Overview
This document describes the implementation of animated dice rolling for the Rogue Dice game. The system adds visual appeal by showing dice cycling through random faces and rotating before settling on their final values.

## New Features Added

### 1. **DieUI Animation Properties**
New configurable properties in `DieUI.cs`:
- `rollAnimationDuration` (1.5s default) - How long the roll animation lasts
- `spriteChangeInterval` (0.08s default) - How often sprites change during animation
- `rotationSpeed` (720°/s default) - Rotation speed during rolling
- `rollingScale` (1.2x default) - Scale factor during animation
- `allDieFaceSprites[]` - Array of all die face sprites for cycling

### 2. **Animation Methods**
- `SetupWithAnimation()` - Main method to setup a die with or without animation
- `PlayRollAnimation()` - Coroutine that handles the rolling animation
- `SetFinalState()` - Sets the final sprite and value after animation
- `IsRolling` property - Tracks if die is currently animating

### 3. **UIManager Enhancements**
- `UpdateDiceUIWithAnimation()` - Updates dice with animation option
- `UpdateRollResult()` - Now supports animation parameter
- `AnyDiceRolling()` - Checks if any dice are still animating
- `WaitForRollingToCompleteAndSetInteractable()` - Waits for animations to finish

### 4. **GameManager Integration**
- `EnableRerollSelectionAfterAnimation()` - Coroutine to enable interaction after animations
- Initial rolls use animation, rerolls are instant for better UX
- Proper timing coordination between animations and user interaction

## Animation Behavior

### Initial Dice Rolls
1. Dice scale up to `rollingScale` (1.2x by default)
2. Sprites cycle randomly through all die faces every `spriteChangeInterval` (0.08s)
3. Dice rotate continuously at `rotationSpeed` (720°/s)
4. After `rollAnimationDuration` (1.5s), dice settle to final values
5. Scale and rotation return to normal
6. User interaction is enabled after all animations complete

### Rerolls
- Rerolls use instant updates (no animation) for faster gameplay
- Only selected dice are updated
- Immediate interaction availability

### Interaction Prevention
- Dice cannot be clicked while rolling (`isRolling` state)
- `SetInteractable()` respects rolling state
- UI waits for animations to complete before enabling selection

## Unity Setup Requirements

### DieUI Component Setup
For each DieUI component in the Inspector:

1. **Animation Settings:**
   - `Roll Animation Duration`: 1.5 (or desired duration)
   - `Sprite Change Interval`: 0.08 (faster = more frantic)
   - `Rotation Speed`: 720 (degrees per second)
   - `Rolling Scale`: 1.2 (scale during animation)

2. **Sprite Assignment:**
   - `All Die Face Sprites`: Assign all 6 die face sprites (1-6)
   - This array is used for random cycling during animation

### UIManager Setup
- Ensure `dieFaceSprites[]` is properly assigned (used for both animation and final display)
- The system automatically assigns `allDieFaceSprites` to each die if not already set

## Technical Details

### Animation Coordination
- Uses Unity Coroutines for smooth animations
- `Time.time` based timing for consistent animation speed
- Proper cleanup of coroutines to prevent memory leaks

### Performance Considerations
- Animations stop immediately when interrupted (e.g., new roll)
- Efficient sprite cycling using array indexing
- Animation state tracking prevents unnecessary updates

### Visual Effects
- Smooth scale interpolation using original transform scale
- Continuous rotation with proper angle wrapping
- Random sprite selection for authentic rolling feel

## Customization Options

### Animation Timing
- Adjust `rollAnimationDuration` for longer/shorter rolls
- Modify `spriteChangeInterval` for different cycling speeds
- Change `rotationSpeed` for faster/slower rotation

### Visual Style
- Modify `rollingScale` for more/less dramatic size changes
- Adjust `selectedScale` for selection feedback
- Customize colors and overlays as desired

### Gameplay Integration
- Initial rolls: Animated for excitement
- Rerolls: Instant for efficiency
- Easy to toggle animation on/off per situation

## Future Enhancements

Potential improvements for the animation system:
1. **Sound Integration**: Sync audio with visual rolling
2. **Particle Effects**: Add sparkles or dust during rolling
3. **Physics Simulation**: More realistic tumbling motion
4. **Individual Timing**: Different animation durations per die
5. **Bounce Effects**: Landing animation when settling

## Files Modified

1. **DieUI.cs** - Core animation logic and timing
2. **UIManager.cs** - Animation coordination and UI updates  
3. **GameManager.cs** - Game flow integration and timing
4. **DiceRollAnimation_Implementation.md** - This documentation

The implementation maintains backward compatibility while adding engaging visual feedback to the dice rolling experience.
