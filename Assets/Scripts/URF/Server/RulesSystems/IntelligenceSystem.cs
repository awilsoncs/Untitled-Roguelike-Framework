namespace URF.Server.RulesSystems {
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Algorithms;
  using URF.Server.GameState;

  // todo need to make this require movement and combat systems
  public class IntelligenceSystem : BaseRulesSystem {

    private IFieldOfView fov;

    private IPathfinding pathfinding;

    private IEntity mainCharacter;

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

      foreach (IEntity entity in this.GameState.GetAllEntities()) {
        ControlMode mode = entity.ControlMode;
        switch (mode) {
          case ControlMode.Monster:
            this.UpdateEntity(entity);
            break;
          case ControlMode.None:
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
      Position mainPosition = this.GameState.LocateEntityOnMap(this.mainCharacter);
      Position entityPosition = this.GameState.LocateEntityOnMap(entity);
      bool[,] transparency = new bool[
        this.GameState.MapSize.X,
        this.GameState.MapSize.Y
      ];
      transparency.Populate((x, y) => this.GameState.GetCell((x, y)).IsTransparent);
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
      (int width, int height) = gs.MapSize;
      float[][] costs = new float[width][];
      for (int x = 0; x < width; x++) {
        costs[x] = new float[height];
        for (int y = 0; y < height; y++) {
          Cell cell = gs.GetCell((x, y));
          costs[x][y] = cell.IsTraversable ? 0.1f : 10000f;
        }
      }
      return costs;
    }

  }
}
