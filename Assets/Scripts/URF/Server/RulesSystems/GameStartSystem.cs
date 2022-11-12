using URF.Common.GameEvents;
using URF.Server.DungeonBuilding;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class GameStartSystem : BaseRulesSystem {

    [EventHandler(GameEventType.StartGameCommand)]
    public void HandleGameStartCommand(IGameState gs, IGameEvent cm) {
      if(gs.Random == null) {
        gs.PostError("Cannot begin game without RandomRandom.");
        return;
      }
      if(gs.FieldOfView == null) {
        gs.PostError("Cannot begin game without FOV plugin.");
        return;
      }
      DungeonBuilder.Build(gs, gs.Random);
      gs.RecalculateFOVImmediately();
      gs.PostEvent(new GameStartedEvent());
    }

  }
}
