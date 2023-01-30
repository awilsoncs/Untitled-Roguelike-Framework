namespace URF.Server.RulesSystems {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.EntityFactory;
  using URF.Algorithms;

  public class GameStartSystem : BaseRulesSystem {

    private IRandomGenerator random;
    private IEntityFactory<Entity> entityFactory;

    private int MapWidth => this.GameState.MapSize.X;
    private int MapHeight => this.GameState.MapSize.Y;
    private Position MapCenter => (this.MapWidth / 2, this.MapHeight / 2);

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.random = pluginBundle.Random;
      this.entityFactory = pluginBundle.EntityFactory;
    }

    public override void HandleConfigure(ConfigureAction _) {
      this.OnGameEvent(new GameConfigured(this.GameState));
      this.BuildDungeon();
      this.OnGameEvent(new GameStarted());
    }

    private void BuildDungeon() {
      for (int i = 0; i < this.MapWidth; i++) {
        _ = this.PutEntity("wall", (i, 0));
        _ = this.PutEntity("wall", (i, this.MapHeight - 1));
      }

      for (int i = 1; i < this.MapHeight - 1; i++) {
        _ = this.PutEntity("wall", (0, i));
        _ = this.PutEntity("wall", (this.MapWidth - 1, i));
      }

      IEntity player = this.PutEntity("player", this.MapCenter);
      this.OnGameEvent(new MainCharacterChanged(player));

      for (int i = 0; i < 4; i++) {
        int x = this.random.GetInt(1, this.MapWidth - 2);
        int y = this.random.GetInt(1, this.MapHeight - 2);
        _ = this.PutEntity("crab", (x, y));
      }

      for (int i = 0; i < 4; i++) {
        int x = this.random.GetInt(1, this.MapWidth - 2);
        int y = this.random.GetInt(1, this.MapHeight - 2);
        _ = this.PutEntity("wall", (x, y));
      }

      for (int i = 0; i < 2; i++) {
        int x = this.random.GetInt(1, this.MapWidth - 2);
        int y = this.random.GetInt(1, this.MapHeight - 2);
        _ = this.PutEntity("scrollOfLightning", (x, y));
      }
    }

    private IEntity PutEntity(string bluePrint, Position position) {
      IEntity entity = this.entityFactory.Get(bluePrint);
      this.GameState.CreateEntity(entity);
      this.GameState.PlaceEntityOnMap(entity, position);
      return entity;
    }

  }
}
