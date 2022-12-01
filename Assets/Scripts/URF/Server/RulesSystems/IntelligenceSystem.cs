namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.Persistence;
  using URF.Server.FieldOfView;
  using URF.Server.GameState;
  using URF.Server.Pathfinding;

  public enum IntelligenceControlMode {

    // NOTE: player is controlled externally, use control mode None.
    None,

    Monster

  }

  // todo need to make this require movement and combat systems
  public class IntelligenceSystem : BaseRulesSystem {

    private IFieldOfView fov;

    private IPathfinding pathfinding;

    private IEntity mainCharacter;

    // todo see notes below
    // After implementing the turn controller, convert this system
    // to handle a command emitted by the turn controller.
    public override List<Type> Components => new() { typeof(Brain) };

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.fov = pluginBundle.FieldOfView;
      this.pathfinding = pluginBundle.Pathfinding;
    }

    public override void HandleMainCharacterChanged(MainCharacterChanged ev) {
      this.mainCharacter = ev.Entity;
    }

    public override void HandleTurnSpent(TurnSpent turnSpent) {
      if (turnSpent.Entity != this.mainCharacter) {
        return;
      }

      foreach (IEntity entity in this.GameState.GetEntities()) {
        IntelligenceControlMode mode = entity.GetComponent<Brain>().ControlMode;
        switch (mode) {
          case IntelligenceControlMode.Monster:
            this.UpdateEntity(entity);
            break;
          case IntelligenceControlMode.None:
            break;
          default:
            this.OnGameEvent(new GameErrored($"Forgot to support intelligence mode {mode}"));
            break;
        }
      }
    }

    private void UpdateEntity(IEntity entity) {
      // todo handle can't move
      // todo handle can't reach target
      Movement mainMovement = this.mainCharacter.GetComponent<Movement>();
      Position mainPosition = mainMovement.EntityPosition;

      Movement entityMovement = entity.GetComponent<Movement>();
      Position entityPosition = entityMovement.EntityPosition;
      bool[,] transparency = new bool[this.GameState.MapWidth, this.GameState.MapHeight];
      for (int x = 0; x < this.GameState.MapWidth; x++) {
        for (int y = 0; y < this.GameState.MapHeight; y++) {
          transparency[x, y] = this.GameState.IsTransparent((x, y));
        }
      }
      if (!this.fov.IsVisible(transparency, entityPosition, mainPosition)) {
        // entity can't see the player, just dawdle.
        return;
      }

      float[][] costs = GetMovementCosts(this.GameState);
      // take a step along the path
      List<Position> path = this.pathfinding.GetPath(costs, entityPosition, mainPosition);
      if (path.Count == 2) {
        // just the start and end means adjacent
        this.OnGameEvent(new AttackAction(entity, this.mainCharacter));
        return;
      }

      // calculate the direction to step
      Position nextStep = path[1];
      (int, int) mp = (nextStep.X - entityPosition.X, nextStep.Y - entityPosition.Y);
      this.OnGameEvent(new MoveAction(entity, mp));
    }

    private static float[][] GetMovementCosts(IGameState gs) {
      (int width, int height) = gs.GetMapSize();
      float[][] costs = new float[width][];
      for (int x = 0; x < width; x++) {
        costs[x] = new float[height];
        for (int y = 0; y < height; y++) {
          costs[x][y] = gs.IsTraversable((x, y)) ? 0.1f : 10000f;
        }
      }
      return costs;
    }

  }

  public class Brain : BaseComponent {

    public IntelligenceControlMode ControlMode {
      get; set;
    }

    public override void Load(IGameDataReader reader) {
      this.ControlMode = (IntelligenceControlMode)reader.ReadInt();
    }

    public override void Save(IGameDataWriter writer) {
      writer.Write((int)this.ControlMode);
    }

  }
}
