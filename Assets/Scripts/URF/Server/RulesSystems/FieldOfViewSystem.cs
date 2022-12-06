namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.FieldOfView;
  using URF.Server.GameState;

  public class FieldOfViewSystem : BaseRulesSystem {

    private IFieldOfView fov;

    private IEntity mainCharacter;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      if (pluginBundle == null) {
        return;
      }
      this.fov = pluginBundle.FieldOfView;
    }

    public override void HandleMainCharacterChanged(MainCharacterChanged ev) {
      // Track the main character so we know where to update the FOV from
      this.mainCharacter = ev.Entity;
    }

    public override void HandleGameStarted(GameStarted _) {
      this.RecalculateFov(this.GameState);
    }

    public override void HandleMoveAction(MoveAction ev) {
      // If it's not the MC, we don't care.
      if (ev.Entity != this.mainCharacter) {
        return;
      }
      this.RecalculateFov(this.GameState);
    }

    private void RecalculateFov(IGameState gs) {
      Position position = this.mainCharacter.GetComponent<Movement>().EntityPosition;
      bool[,] transparency = new bool[gs.MapWidth, gs.MapHeight];
      for (int x = 0; x < gs.MapWidth; x++) {
        for (int y = 0; y < gs.MapHeight; y++) {
          Cell cell = gs.GetCell((x, y));
          transparency[x, y] = cell.IsTransparent;
        }
      }
      IFieldOfViewQueryResult result = this.fov.CalculateFov(transparency, position);
      for (int x = 0; x < gs.MapWidth; x++) {
        for (int y = 0; y < gs.MapHeight; y++) {
          bool isVisible = result.IsVisible((x, y));
          Cell cell = gs.GetCell((x, y));
          foreach (IEntity entity in cell.Contents) {
            entity.IsVisible = isVisible;
            this.OnGameEvent(new EntityVisibilityChanged(entity, isVisible));
          }
        }
      }
    }

  }
}
