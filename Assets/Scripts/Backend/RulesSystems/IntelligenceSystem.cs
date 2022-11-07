using System;
using System.Collections.Generic;
using URFCommon;

public enum IntelligenceControlMode {
    // NOTE: player is controlled externally, use control mode None.
    None,
    Monster
}

// todo need to make this require movement and combat systems
public class IntelligenceSystem : BaseRulesSystem {

    // todo see notes below
    // After implementing the turn controller, convert this system
    // to handle a command emitted by the turn controller.
    public override List<Type> Components => new () {
        typeof(Brain)
    };

    public override void GameUpdate(IGameState gameState) {
        foreach (var entity in gameState.GetEntities()) {

            if (entity.GetComponent<Brain>()
                .ControlMode == IntelligenceControlMode.Monster) {
                UpdateEntity(gameState, entity);
            }
        }
        
    }

    private void UpdateEntity(IGameState gameState, IEntity entity) {
        // todo handle can't move
        // todo handle can't reach target
        var mainCharacter = gameState.GetMainCharacter();
        var mainMovement = mainCharacter.GetComponent<Movement>();
        var mainPosition = mainMovement.Position;

        var entityMovement = entity.GetComponent<Movement>();
        var entityPosition = entityMovement.Position;
        if (!gameState.FieldOfView.IsVisible(gameState, entityPosition, mainPosition)) {
            // entity can't see the player, just dawdle.
            return;
        }

        var costs = GetMovementCosts(gameState);
        // take a step along the path
        var path = gameState.Pathfinding.GetPath(costs, entityPosition, mainPosition);
        if (path.Count == 2) {
            // just the start and end means adjacent
            gameState.PostEvent(
                new AttackCommand(entity.ID, gameState.GetMainCharacter().ID));
            return;
        }

        // calculate the direction to step
        var nextStep = path[1];
        var mp = (nextStep.X - entityPosition.X, nextStep.Y - entityPosition.Y);
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

[Component("312dc848-5f31-42b8-900c-a73289cd40d2")]
public class Brain : BaseComponent
{
    public IntelligenceControlMode ControlMode;
    public override void Load(GameDataReader reader)
    {
        ControlMode = (IntelligenceControlMode)reader.ReadInt();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write((int)ControlMode);
    }
}
