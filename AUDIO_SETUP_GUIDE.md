# Audio Setup Guide for Dice Rolling Sound Effects

## Overview
The dice rolling system now includes sound effects that play during both initial rolls and rerolls. The implementation uses a separate AudioManager component to handle all sound effects, keeping audio concerns separate from dice logic.

## Architecture
- **Dice.cs** - Handles dice rolling logic and calls AudioManager for sound effects
- **AudioManager.cs** - Centralized audio system managing all game sound effects  
- **Separation of Concerns** - Dice focus on gameplay, AudioManager handles all audio

## Files Modified
- ✅ **DieUI.cs** - Removed fade-out effect (dice stay fully opaque)
- ✅ **Dice.cs** - Calls AudioManager for sound effects via GameObject messaging
- ✅ **AudioManager.cs** - Complete audio management system

## Unity Setup Instructions

### Step 1: Create AudioManager GameObject
1. In your scene hierarchy, create an empty GameObject
2. Name it **"AudioManager"** (or any name you prefer)
3. Add the `AudioManager` component to this GameObject

### Step 2: Configure AudioManager Component
In the AudioManager component Inspector:
1. **SFX Audio Source**: Will auto-create if empty, or drag an AudioSource component
2. **Dice Roll SFX**: Drag your `Dice SFX.mp3` file from the SFX folder here
3. **SFX Volume**: Adjust volume (0.0 to 1.0, default is 1.0)
4. **SFX Enabled**: Check to enable sound effects

### Step 3: Dice Component Setup
The Dice component no longer needs any audio configuration - it will automatically find and use the AudioManager in your scene.

## How It Works

### Communication Pattern
- Dice class uses `AudioManager.Instance` singleton to access the audio system
- Calls `audioManager.PlayDiceRollSound()` directly for clean, type-safe communication
- AudioManager singleton pattern ensures easy access from any script

### Sound Effect Triggers
The dice roll sound effect will play in two scenarios:
1. **Initial Roll**: When `Dice.Roll()` is called
2. **Reroll**: When `Dice.RerollDice()` is called

## Testing
1. Ensure AudioManager GameObject exists in your scene
2. Assign the dice SFX audio clip in AudioManager Inspector
3. Enter Play mode in Unity
4. Trigger a dice roll - you should hear the sound effect
5. Try rerolling dice to confirm the sound plays again

## Troubleshooting

### No Sound Playing
Check the Console for these debug messages:
- `"[Dice] Playing dice roll sound effect via AudioManager"` - Sound call successful
- `"[Dice] AudioManager not found - cannot play dice roll sound"` - AudioManager singleton not initialized
- `"[AudioManager] Playing dice roll sound"` - AudioManager received the call

### AudioManager Issues
- Ensure AudioManager GameObject exists in the scene
- Check that AudioManager component is attached to the GameObject
- Verify the dice SFX audio clip is assigned in AudioManager Inspector
- Check SFX Enabled checkbox is checked

### Audio Volume Issues
- Adjust **SFX Volume** in AudioManager Inspector
- Check Unity's master volume settings
- Verify your system audio isn't muted

### Audio File Issues
- Ensure `Dice SFX.mp3` is imported as an AudioClip in Unity
- Check the import settings (right-click → Reimport if needed)
- Verify the file path: `Assets/SFX/Dice SFX.mp3`

## Benefits of This Architecture

### Separation of Concerns
- **Dice.cs**: Focuses purely on dice rolling logic
- **AudioManager.cs**: Handles all audio-related functionality
- Clean interface between gameplay and audio systems

### Scalability
- Easy to add new sound effects to AudioManager
- All audio settings centralized in one place
- Can easily disable all audio from one location

### Flexibility
- AudioManager can be extended for background music
- Easy to implement audio mixing and effects
- Supports future audio features like volume fading

## Future Enhancements
The AudioManager system is ready for expansion:
- Background music support
- Multiple sound effect categories
- Audio settings menu integration
- Sound effect randomization and variations
- Audio mixing and ducking

## Code Structure
```
Dice.cs
├── Dice Logic
│   ├── Roll() - Calls PlayDiceSound()
│   └── RerollDice() - Calls PlayDiceSound()
└── Audio Integration
    └── PlayDiceSound() - Accesses AudioManager.Instance

AudioManager.cs (Singleton)
├── Audio Configuration (Inspector)
│   ├── diceRollSFX (AudioClip)
│   ├── sfxVolume (float)
│   └── sfxEnabled (bool)
├── Audio Components
│   └── sfxAudioSource (AudioSource)
├── Singleton Access
│   └── Instance - Static property for global access
└── Public Methods
    └── PlayDiceRollSound() - Called directly by Dice class
```

This architecture provides clean separation with proper type-safe communication!
