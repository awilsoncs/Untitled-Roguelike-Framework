namespace URF.Server.RulesSystems {
  using System.Diagnostics;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.FieldOfView;
  using URF.Server.GameState;

  public class FieldOfViewSystem : BaseRulesSystem {

    private IFieldOfView fov;

    private IEntity mainCharacter;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      Debug.Assert(pluginBundle != null);
      Debug.Assert(pluginBundle.FieldOfView != null);
      this.fov = pluginBundle.FieldOfView;
    }

    [EventHandler(GameEventType.MainCharacterChanged)]
    public void HandleMainCharacterChanged(IGameState _, IGameEventArgs ev) {
      // Track the main character so we know where to update the FOV from
      var mcc = (MainCharacterChangedEventArgs)ev;
      this.mainCharacter = mcc.Entity;
    }

    [EventHandler(GameEventType.Start)]
    public void HandleGameStart(IGameState gs, IGameEventArgs _) {
      this.RecalculateFov(gs);
    }

    [ActionHandler(GameEventType.MoveCommand)]
    public void HandleMoveAction(IGameState gs, IActionEventArgs cm) {
      var ev = (MoveActionEventArgs)cm;

      // If it's not the MC, we don't care.
      if (ev.EntityId != this.mainCharacter.ID) {
        return;
      }
      this.RecalculateFov(gs);
    }

    private void RecalculateFov(IGameState gs) {
      Position position = this.mainCharacter.GetComponent<Movement>().EntityPosition;
      IFieldOfViewQueryResult result = this.fov.CalculateFOV(gs, position);
      for (int x = 0; x < gs.MapWidth; x++) {
        for (int y = 0; y < gs.MapHeight; y++) {
          bool isVisible = result.IsVisible((x, y));
          Cell cell = gs.GetCell((x, y));
          foreach (IEntity entity in cell.Contents) {
            entity.IsVisible = isVisible;
            this.OnGameEvent(new EntityVisibilityChangedEventArgs(entity, isVisible));
          }
        }
      }
    }

  }
}
