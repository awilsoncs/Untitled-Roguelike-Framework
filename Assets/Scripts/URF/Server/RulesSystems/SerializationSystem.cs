namespace URF.Server.RulesSystems {
  using System.Collections.ObjectModel;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.Persistence;
  using URF.Server.EntityFactory;
  using URF.Server.RandomGeneration;

  public class SerializationSystem : BaseRulesSystem, IPersistableObject {

    private const int SaveVersion = 1;

    private PersistentStorage persistentStorage;

    private IEntityFactory<Entity> entityFactory;

    private IEntity mainCharacter;

    private IRandomGenerator randomGenerator;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      this.persistentStorage = pluginBundle.PersistentStorage;
      this.entityFactory = pluginBundle.EntityFactory;
      this.randomGenerator = pluginBundle.Random;
    }

    public override void HandleMainCharacterChanged(MainCharacterChanged ev) {
      this.mainCharacter = ev.Entity;
    }

    public override void HandleSaveAction(SaveAction _) {
      this.persistentStorage.Save(this, SaveVersion);
    }

    public override void HandleLoad(LoadAction _) {
      this.persistentStorage.Load(this);
    }

    public void Save(IGameDataWriter writer) {
      this.randomGenerator.Save(writer);
      ReadOnlyCollection<IEntity> entities = this.GameState.GetEntities();
      writer.Write(entities.Count);
      writer.Write(this.mainCharacter.ID);
      foreach (IEntity entity in entities) {
        writer.Write(entity.ID);
        entity.Save(writer);
      }
    }

    public void Load(IGameDataReader reader) {
      this.randomGenerator.Load(reader);
      Position mapSize = (this.GameState.MapWidth, this.GameState.MapHeight);
      this.OnGameEvent(new GameConfigured(mapSize));
      int count = reader.ReadInt();
      int mainCharacterId = reader.ReadInt();
      for (int i = 0; i < count; i++) {
        int entityID = reader.ReadInt();
        IEntity entity = this.entityFactory.Get();
        entity.ID = entityID;
        entity.Load(reader);
        Position position = entity.GetComponent<Movement>().EntityPosition;
        this.GameState.CreateEntityAtPosition(entity, position);
        this.OnGameEvent(new EntityCreated(entity));
        this.OnGameEvent(new EntityMoved(entity, position));
        if (entityID == mainCharacterId) {
          this.OnGameEvent(new MainCharacterChanged(entity));
        }

      }
    }

  }
}

