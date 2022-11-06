using System.Collections.Generic;
using URFCommon;

public class IntelligenceSystem : BaseRulesSystem {

    // todo see notes below
    // After implementing the turn controller, convert this system
    // to handle a command emitted by the turn controller.
    public override List<(string, SlotType)> Slots => new () {
        // valid values are "player", "monster"
        ("controlMethod", SlotType.String),
        ("X", SlotType.Integer),
        ("Y", SlotType.Integer)
    };

    public override void GameUpdate(IGameState gameState)
    {
        foreach (var entity in gameState.GetEntities()) {
            if (entity.GetStringSlot("controlMethod") == "monster") {
                UpdateEntity(gameState, entity);
            }
        }
        
    }

    private void UpdateEntity(IGameState gameState, IEntity entity) {
        // todo make this only happen if the entity can see the player
        var mainCharacter = gameState.GetMainCharacter();
        var mainCharacterPosition = (
            mainCharacter.GetIntSlot("X"), mainCharacter.GetIntSlot("Y"));
        Position position = (entity.GetIntSlot("X"), entity.GetIntSlot("Y"));
        if (!gameState.FieldOfView.IsVisible(gameState, position, mainCharacterPosition)) {
            // entity can't see the player, just dawdle.
            return;
        }

        var costs = GetMovementCosts(gameState);
        // take a step along the path
        var path = gameState.Pathfinding.GetPath(costs, position, mainCharacterPosition);
        // todo if we're close just attack
        if (path.Count == 2) {
            // just the start and end means adjacent
            gameState.PostEvent(new AttackCommand(entity.ID, gameState.GetMainCharacter().ID));
            return;
        }
        var nextStep = path[1];
        var mp = (nextStep.X - position.X, nextStep.Y - position.Y);
        gameState.PostEvent(new MoveCommand(entity.ID, mp));
    }

    private float[][] GetMovementCosts(IGameState gs)
    {
        (int width, int height) = gs.GetMapSize();
        float[][] costs = new float[width][];
        for (int x = 0; x < width; x++)
        {
            costs[x] = new float[height];
            for (int y = 0; y < height; y++)
            {
                costs[x][y] = gs.IsTraversable((x, y)) ? 0.1f : 10000f;
            }
        }
        return costs;
    }
}