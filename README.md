# Untitled Roguelike Framework
URF is a flexible framework for building turn-based, tile-based games in Unity.

## Project Structure
URF is separated into a back end and a front end.

### Plugins
Here you will find a collection of plugins for modifying framework behavior.

### Scripts / Backend
The backend defines gameplay rules handling for the framework. The core of it is defined in `Assets\Scripts\Backend\GameState\GameState.cs`.

#### Plugins
You can begin your journey exploring gameplay plugins at `Assets\Scripts\Backend\FrameworkPlugins`.

#### Entities & EntityParts
This pair of classes, found in `Assets\Scripts\Backend\Entities` and `Assets\Scripts\Backend\EntityParts` respectively, define the true gameplay logic of the game. The framework consumer will define a database of entities and create a bespoke DungeonBuilder* to push entities into the world.

*Name subject to change.

### Scripts / Frontend
The frontend can be thought of as the Unity integration point. It provides three services (so far):

1. User input handling
2. Game state rendering
3. Backend configuration

### User Input Handling (todo)
User input is currently hardcoded and will eventually be migrated to a client plugin. You can find this logic in `Assets\Scripts\Frontend\Game.UserInput.cs`.

### Game state rendering
The framework renders the game state by instantiating a collection of pawns. You can find this logic in `Assets\Scripts\Frontend\Game.cs`.

### Backend configuration
The client script also serves as a game setup script. In the game controller inspector, you will find a drawer for Backend plugins. These plugins are defined in `Assets\Scripts\Backend\FrameworkPlugins`. Each functions by referencing a snippet of backend logic that injects into the backend during game state initialization.

### Scripts / Shared (todo)
Currently, the communication interface between the front and back ends lives on the backend side. Eventually, these will be migrated to a third location separate from both.
