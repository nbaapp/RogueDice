# Perk Selection System Setup Guide

## Overview
The perk selection system allows players to choose from 3 random perks after completing each round. If the player has reached their maximum perk capacity, they will be prompted to replace one of their existing perks.

## Components Added

### 1. PerkManager.cs
- Manages the perk database and player perk inventory
- Handles perk selection logic and random generation
- Configurable max perk count and choice count

### 2. PerkSelectionUI.cs
- UI manager for perk selection and replacement interfaces
- Handles both new perk selection and perk replacement flows
- Uses PerkCardUI components for displaying perk choices

### 3. GameManager Updates
- Integrated perk selection into round completion flow
- Added event handling for perk selection/replacement
- Prevents actions during perk selection state

### 4. PerkSystemTester.cs (Optional)
- Testing utility for debugging the perk system
- Provides keyboard shortcuts and GUI for testing
- Useful for validating functionality during development

## Setup Instructions

### 1. Create GameObjects
1. Create an empty GameObject named "PerkManager" and attach the PerkManager script
2. Create an empty GameObject named "PerkSelectionUI" and attach the PerkSelectionUI script
3. (Optional) Create a "PerkSystemTester" GameObject for testing

### 2. Configure PerkManager
1. Assign all available perk assets to the "All Available Perks" array
2. Set "Max Player Perks" (default: 5)
3. Set "Perk Choice Count" (default: 3)

### 3. Create Perk Selection UI
1. Create a main selection panel with:
   - Title text (TextMeshPro)
   - 3 perk choice cards (each with PerkCardUI script)
   - 3 corresponding buttons for selection
   - Cancel button (optional)

2. Create a replacement panel with:
   - Title text (TextMeshPro)
   - Perk cards for current player perks (PerkCardUI scripts)
   - Corresponding buttons for replacement selection
   - Cancel button (optional)

### 4. Configure PerkSelectionUI
1. Assign the selection panel GameObject
2. Assign the replacement panel GameObject
3. Assign the title text components
4. Assign the PerkCardUI arrays for both choice and player perk cards
5. Assign the button arrays for both selection types
6. Assign the cancel button

### 5. Update GameManager
1. Assign PerkManager reference
2. Assign PerkSelectionUI reference
3. The existing activePerks array will be automatically synchronized

## Usage Flow

1. Player completes a round by reaching the target score
2. GameManager triggers perk selection
3. PerkManager generates 3 random unique perk choices
4. If player can add more perks:
   - Show perk selection UI with 3 choices
   - Player clicks a choice to select it
5. If player is at max capacity:
   - Show first perk choice for replacement
   - Display current player perks
   - Player clicks which perk to replace
6. Selected perk is added to player's collection
7. Game continues to next round

## Testing

Use PerkSystemTester for debugging:
- **P Key**: Show perk selection UI
- **A Key**: Add random perk to collection
- **C Key**: Clear all player perks
- **Inspector Toggles**: Same functionality via checkboxes

## Notes

- Perks are applied in order during score calculation
- The system prevents rolling dice during perk selection
- Perk selection can be cancelled (skips the reward)
- All perk operations include comprehensive debug logging
- The UI supports hover effects and rarity-based coloring

## Integration with Existing Code

The system integrates seamlessly with existing:
- Dice rolling and confirmation system
- Score calculation with perk modifiers
- Round progression and game state management
- UI updates and event system
