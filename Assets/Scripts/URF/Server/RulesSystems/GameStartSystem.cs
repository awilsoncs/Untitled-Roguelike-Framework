using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.GameState;
using URF.Server.RandomGeneration;

namespace URF.Server.RulesSystems {
  public class GameStartSystem : BaseRulesSystem {

    private IRandomGenerator _random;
    private IEntityFactory _entityFactory;
    
    [ActionHandler(GameEventType.StartGame)]
    public void HandleGameStartCommand(IGameState gs, IActionEventArgs cm) {
      BuildDungeon(gs, _random);
      OnGameEvent(new GameStartedEventArgs((gs.MapWidth, gs.MapWidth)));
    }

    private void PutEntity(IGameState gs, string bluePrint, Position position) {
      IEntity entity = _entityFactory.Get(bluePrint);
      gs.CreateEntityAtPosition(entity, position);
      OnGameEvent(new EntityCreatedEventArgs(entity));
    }

    private void BuildDungeon(IGameState gs, IRandomGenerator rng) {
      for(int i = 0; i < gs.MapWidth; i++) {
        PutEntity(gs, "wall", (i, 0));
        PutEntity(gs, "wall", (i, gs.MapHeight - 1));
      }

      for(int i = 1; i < gs.MapHeight - 1; i++) {
        PutEntity(gs, "wall", (0, i));
        PutEntity(gs, "wall", (gs.MapWidth - 1, i));
      }

      IEntity player = _entityFactory.Get("player");
      gs.CreateEntityAtPosition(player, (gs.MapWidth / 2, gs.MapHeight / 2));
      OnGameEvent(new EntityCreatedEventArgs(player));
      OnGameEvent(new MainCharacterChangedEventArgs(player));
      
      for(int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        PutEntity(gs, "crab", (x, y));
      }

      for(int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        PutEntity(gs, "wall", (x, y));
      }

      for(int i = 0; i < 2; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        PutEntity(gs, "healthPotion", (x, y));
      }
    }
    
  }
}
