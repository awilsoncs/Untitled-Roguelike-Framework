namespace URF.Server.RulesSystems {
  using System.Collections.Generic;
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
      IReadOnlyCollection<IEntity> entities = this.GameState.GetAllEntities();
      writer.Write(entities.Count);
      writer.Write(this.mainCharacter.ID);
      foreach (IEntity entity in entities) {
        writer.Write(entity.ID);
        Position entityPosition = this.GameState.LocateEntityOnMap(entity);
        writer.Write(entityPosition.X);
        writer.Write(entityPosition.Y);
        entity.Save(writer);
      }
    }

    public void Load(IGameDataReader reader) {
      this.randomGenerator.Load(reader);
      Position mapSize = (this.GameState.MapSize.X, this.GameState.MapSize.Y);
      this.OnGameEvent(new GameConfigured(mapSize));
      int count = reader.ReadInt();
      int mainCharacterId = reader.ReadInt();
      for (int i = 0; i < count; i++) {
        int entityID = reader.ReadInt();
        IEntity entity = this.entityFactory.Get();
        entity.ID = entityID;
        var entityPosition = new Position(reader.ReadInt(), reader.ReadInt());

        entity.Load(reader);
        // todo save and load location
        this.GameState.CreateEntity(entity);
        if (!Position.Invalid.Equals(entityPosition)) {
          this.GameState.PlaceEntityOnMap(entity, entityPosition);
        }
        if (entityID == mainCharacterId) {
          this.OnGameEvent(new MainCharacterChanged(entity));
        }

      }
    }

  }
}

