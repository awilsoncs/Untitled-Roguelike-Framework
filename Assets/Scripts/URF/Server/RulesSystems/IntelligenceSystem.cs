using System;
using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.FieldOfView;
using URF.Server.GameState;
using URF.Server.Pathfinding;

namespace URF.Server.RulesSystems {
  public enum IntelligenceControlMode {

    // NOTE: player is controlled externally, use control mode None.
    None,

    Monster

  }

  // todo need to make this require movement and combat systems
  public class IntelligenceSystem : BaseRulesSystem {

    private IFieldOfView _fov;

    private IPathfinding _pathfinding;

    private IEntity _mainCharacter;

    // todo see notes below
    // After implementing the turn controller, convert this system
    // to handle a command emitted by the turn controller.
    public override List<Type> Components => new() { typeof(Brain) };

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _fov = pluginBundle.FieldOfView;
      _pathfinding = pluginBundle.Pathfinding;
    }

    [EventHandler(GameEventType.MainCharacterChanged)]
    public void HandleMainCharacterChanged(IGameState gameState, IGameEventArgs ev) {
      MainCharacterChangedEventArgs mcc = (MainCharacterChangedEventArgs)ev;
      _mainCharacter = mcc.Entity;
    }

    [EventHandler(GameEventType.SpentTurn)]
    public void RunAI(IGameState gameState, IGameEventArgs ev) {
      TurnSpentEventArgs turnSpent = (TurnSpentEventArgs)ev;
      if(turnSpent.Entity != _mainCharacter) { return; }

      foreach(IEntity entity in gameState.GetEntities()) {
        IntelligenceControlMode mode = entity.GetComponent<Brain>().ControlMode;
        switch(mode) {
          case IntelligenceControlMode.Monster:
            UpdateEntity(gameState, entity);
            break;
          case IntelligenceControlMode.None:
            break;
          default:
            OnGameEvent(new GameErroredEventArgs($"Forgot to support intelligence mode {mode}"));
            break;
        }
      }
    }

    private void UpdateEntity(IGameState gameState, IEntity entity) {
      // todo handle can't move
      // todo handle can't reach target
      Movement mainMovement = _mainCharacter.GetComponent<Movement>();
      Position mainPosition = mainMovement.EntityPosition;

      Movement entityMovement = entity.GetComponent<Movement>();
      Position entityPosition = entityMovement.EntityPosition;
      if(!_fov.IsVisible(gameState, entityPosition, mainPosition)) {
        // entity can't see the player, just dawdle.
        return;
      }

      float[][] costs = GetMovementCosts(gameState);
      // take a step along the path
      List<Position> path = _pathfinding.GetPath(costs, entityPosition, mainPosition);
      if(path.Count == 2) {
        // just the start and end means adjacent
        OnGameAction(new AttackActionEventArgs(entity.ID, _mainCharacter.ID));
        return;
      }

      // calculate the direction to step
      Position nextStep = path[1];
      (int, int) mp = (nextStep.X - entityPosition.X, nextStep.Y - entityPosition.Y);
      OnGameAction(new MoveActionEventArgs(entity.ID, mp));
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

    public override void Load(IGameDataReader reader) {
      ControlMode = (IntelligenceControlMode)reader.ReadInt();
    }

    public override void Save(IGameDataWriter writer) {
      writer.Write((int)ControlMode);
    }

  }
}
