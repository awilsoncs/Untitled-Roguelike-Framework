namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Algorithms;
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

    public override void HandleEntityLocationChanged(EntityLocationChanged entityLocationChanged) {
      // If it's not the MC, we don't care.
      if (
        entityLocationChanged.Entity != this.mainCharacter
        || entityLocationChanged.SubType is not EntityLocationChanged.EventSubType.Moved
      ) {
        return;
      }
      this.RecalculateFov(this.GameState);
    }

    private void RecalculateFov(IGameState gs) {
      Position position = this.GameState.LocateEntityOnMap(this.mainCharacter);

      int mapWidth = this.GameState.MapSize.X;
      int mapHeight = this.GameState.MapSize.Y;
      bool[,] transparency = new bool[mapWidth, mapHeight];
      for (int x = 0; x < mapWidth; x++) {
        for (int y = 0; y < mapHeight; y++) {
          Cell cell = gs.GetCell((x, y));
          transparency[x, y] = cell.IsTransparent;
        }
      }
      IFieldOfViewQueryResult result = this.fov.CalculateFov(transparency, position);
      for (int x = 0; x < mapWidth; x++) {
        for (int y = 0; y < mapHeight; y++) {
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
