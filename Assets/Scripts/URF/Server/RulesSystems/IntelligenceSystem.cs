using System;
using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
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
    public override List<Type> Components => new() { typeof(Brain) };

    public override void GameUpdate(IGameState gameState) {
      foreach(IEntity entity in gameState.GetEntities()) {
        IntelligenceControlMode mode = entity.GetComponent<Brain>().ControlMode;
        switch(mode) {
          case IntelligenceControlMode.Monster:
            UpdateEntity(gameState, entity);
            break;
          case IntelligenceControlMode.None:
            break;
          default:
            gameState.Log($"Forgot to support intelligence mode {mode}");
            break;
        }
      }
    }

    private void UpdateEntity(IGameState gameState, IEntity entity) {
      // todo handle can't move
      // todo handle can't reach target
      IEntity mainCharacter = gameState.GetMainCharacter();
      Movement mainMovement = mainCharacter.GetComponent<Movement>();
      Position mainPosition = mainMovement.EntityPosition;

      Movement entityMovement = entity.GetComponent<Movement>();
      Position entityPosition = entityMovement.EntityPosition;
      if(!gameState.FieldOfView.IsVisible(gameState, entityPosition, mainPosition)) {
        // entity can't see the player, just dawdle.
        return;
      }

      float[][] costs = GetMovementCosts(gameState);
      // take a step along the path
      List<Position> path = gameState.Pathfinding.GetPath(costs, entityPosition, mainPosition);
      if(path.Count == 2) {
        // just the start and end means adjacent
        gameState.PostEvent(new AttackCommand(entity.ID, gameState.GetMainCharacter().ID));
        return;
      }

      // calculate the direction to step
      Position nextStep = path[1];
      (int, int) mp = (nextStep.X - entityPosition.X, nextStep.Y - entityPosition.Y);
      gameState.PostEvent(new MoveCommand(entity.ID, mp));
    }

    private static float[][] GetMovementCosts(IGameState gs) {
      (int width, int height) = gs.GetMapSize();
      float[][] costs = new float[width][];
      for(int x = 0; x < width; x++) {
        costs[x] = new float[height];
        for(int y = 0; y < height; y++) { costs[x][y] = gs.IsTraversable((x, y)) ? 0.1f : 10000f; }
      }
      return costs;
    }

  }

  public class Brain : BaseComponent {

    public IntelligenceControlMode ControlMode { get; set; }

    public override void Load(GameDataReader reader) {
      ControlMode = (IntelligenceControlMode)reader.ReadInt();
    }

    public override void Save(GameDataWriter writer) {
      writer.Write((int)ControlMode);
    }

  }
}
