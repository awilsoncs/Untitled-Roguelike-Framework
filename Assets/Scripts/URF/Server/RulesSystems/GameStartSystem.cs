namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.EntityFactory;
  using URF.Server.GameState;
  using URF.Server.RandomGeneration;

  public class GameStartSystem : BaseRulesSystem {

    private IRandomGenerator random;
    private IEntityFactory<Entity> entityFactory;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.random = pluginBundle.Random;
      this.entityFactory = pluginBundle.EntityFactory;
    }

    public override void HandleConfigure(ConfigureAction _) {
      this.OnGameEvent(new GameConfigured((this.GameState.MapWidth, this.GameState.MapHeight)));
      this.BuildDungeon(this.GameState, this.random);
      this.OnGameEvent(new GameStarted());
    }

    private void BuildDungeon(IGameState gs, IRandomGenerator rng) {
      for (int i = 0; i < gs.MapWidth; i++) {
        _ = this.PutEntity(gs, "wall", (i, 0));
        _ = this.PutEntity(gs, "wall", (i, gs.MapHeight - 1));
      }

      for (int i = 1; i < gs.MapHeight - 1; i++) {
        _ = this.PutEntity(gs, "wall", (0, i));
        _ = this.PutEntity(gs, "wall", (gs.MapWidth - 1, i));
      }

      IEntity player = this.PutEntity(gs, "player", (gs.MapWidth / 2, gs.MapHeight / 2));
      this.OnGameEvent(new MainCharacterChanged(player));

      for (int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        _ = this.PutEntity(gs, "crab", (x, y));
      }

      for (int i = 0; i < 4; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        _ = this.PutEntity(gs, "wall", (x, y));
      }

      for (int i = 0; i < 2; i++) {
        int x = rng.GetInt(1, gs.MapWidth - 2);
        int y = rng.GetInt(1, gs.MapHeight - 2);
        _ = this.PutEntity(gs, "healthPotion", (x, y));
      }
    }

    private IEntity PutEntity(IGameState gs, string bluePrint, Position position) {
      IEntity entity = this.entityFactory.Get(bluePrint);
      gs.CreateEntityAtPosition(entity, position);
      this.OnGameEvent(new EntityCreated(entity));
      this.OnGameEvent(new EntityMoved(entity, position));
      return entity;
    }

  }
}
