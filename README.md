# Rogue Dice

A Unity-based dice rolling game with roguelike elements, featuring animated dice rolls, perks system, and progressive difficulty.

## Features

### 🎲 **Animated Dice Rolling**
- Dynamic rolling animations with sprite cycling
- Rotation and scaling effects during rolls
- Smooth transitions between roll states
- Configurable animation timing and effects

### 🎮 **Gameplay Mechanics**
- **Progressive Rounds**: Each round increases the target score
- **Reroll System**: Limited rerolls per round for strategic gameplay
- **Perk System**: Collect and manage perks that modify dice scoring
- **Resource Management**: Balance rolls and rerolls to achieve targets

### 🔧 **Technical Features**
- **Modular Architecture**: Clean separation of game logic, UI, and dice mechanics
- **Event-Driven Design**: Unity Events for loose coupling between systems
- **Configurable Settings**: Easily adjustable game parameters
- **Debug Support**: Comprehensive logging and testing utilities

## Project Structure

```
Assets/
├── Scripts/
│   ├── GameManager.cs          # Core game state and flow
│   ├── UIManager.cs            # UI coordination and updates  
│   ├── DieUI.cs               # Individual die animation and interaction
│   ├── Dice.cs                # Dice rolling logic
│   ├── PerkManager.cs         # Perk system management
│   └── Testing/               # Test utilities and validation
├── Scenes/                    # Unity scenes
├── Prefabs/                   # Reusable game objects
├── UI/                        # UI assets and layouts
└── Art/                       # Visual assets and sprites
```

## Getting Started

### Prerequisites
- Unity 6 or later
- Git for version control

### Setup
1. Clone this repository
2. Open the project in Unity
3. Open the main scene from `Assets/Scenes/`
4. Configure dice sprites and UI references in the Inspector
5. Play to start rolling!

### Animation Setup
The dice animation system requires proper sprite assignment:

1. **DieUI Components**: Assign all die face sprites (1-6) to the `allDieFaceSprites` array
2. **Animation Settings**: Configure timing, rotation speed, and scaling in the Inspector
3. **UIManager**: Ensure `dieFaceSprites` array is properly assigned

See `DiceRollAnimation_Implementation.md` for detailed setup instructions.

## Development

### Key Systems

**GameManager**: Orchestrates game flow, round progression, and scoring
**UIManager**: Handles all UI updates and player interaction coordination  
**DieUI**: Manages individual die animations and selection states
**Dice**: Core dice rolling mechanics and reroll functionality
**PerkManager**: Handles perk selection, application, and management

### Testing
The project includes test utilities in `Assets/Scripts/Testing/` for validating:
- Dice rolling and reroll mechanics
- UI interaction states
- Animation timing and coordination
- Perk system functionality

## Built With

- **Unity 6** - Game engine
- **C#** - Programming language
- **Unity UI** - User interface system
- **Unity Events** - Event system for decoupled architecture

## Contributing

This is a personal learning project showcasing Unity development best practices including:
- Clean architecture patterns
- Coroutine-based animation systems
- Event-driven programming
- Modular component design

## License

This project is for educational and portfolio purposes.

## Acknowledgments

- Unity Technologies for the excellent game engine
- Community tutorials and documentation that helped shape this implementation
