using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.FieldOfView;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class FieldOfViewSystem : BaseRulesSystem {

    private IFieldOfView _fov;

    private IEntity _mainCharacter;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _fov = pluginBundle.FieldOfView;
    }

    [EventHandler(GameEventType.MainCharacterChanged)]
    public void HandleMainCharacterChanged(IGameState gs, IGameEventArgs ev) {
      // Track the main character so we know where to update the FOV from
      MainCharacterChangedEventArgs mcc = (MainCharacterChangedEventArgs)ev;
      _mainCharacter = mcc.Entity;
    }

    [ActionHandler(GameEventType.MoveCommand)]
    public void HandleDebugAction(IGameState gs, IActionEventArgs cm) {
      MoveActionEventArgs ev = (MoveActionEventArgs)cm;

      // If it's not the MC, we don't care.
      if(ev.EntityId != _mainCharacter.ID) { return; }

      IFieldOfViewQueryResult result
        = _fov.CalculateFOV(gs, _mainCharacter.GetComponent<Movement>().EntityPosition);
      for(int x = 0; x < gs.MapWidth; x++) {
        for(int y = 0; y < gs.MapHeight; y++) {
          bool isVisible = result.IsVisible((x, y));
          var cell = gs.GetCell((x, y));
          foreach(var entity in cell.Contents) {
            entity.IsVisible = isVisible;
            OnGameEvent(new EntityVisibilityChangedEventArgs(entity, isVisible));
          }
        }
      }
    }

  }
}
