using System.Collections.ObjectModel;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.EntityFactory;
using URF.Server.GameState;
using URF.Server.RandomGeneration;

namespace URF.Server.RulesSystems {
  public class SerializationSystem : BaseRulesSystem, IPersistableObject {

    private const int saveVersion = 1;

    private PersistentStorage _persistentStorage;

    private IEntityFactory<Entity> _entityFactory;

    private IEntity _mainCharacter;

    private IGameState _gameState;

    private IRandomGenerator _randomGenerator;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _persistentStorage = pluginBundle.PersistentStorage;
      _entityFactory = pluginBundle.EntityFactory;
      _randomGenerator = pluginBundle.Random;
    }

    [EventHandler(GameEventType.MainCharacterChanged)]
    public void HandleMainCharacterChanged(IGameState gs, IGameEventArgs ev) {
      MainCharacterChangedEventArgs mcc = (MainCharacterChangedEventArgs)ev;
      _mainCharacter = mcc.Entity;
    }

    [ActionHandler(GameEventType.Save)]
    public void HandleSaveAction(IGameState gs, IActionEventArgs cm) {
      _gameState = gs;
      _persistentStorage.Save(this, saveVersion);
    }

    [ActionHandler(GameEventType.Load)]
    public void HandleLoadAction(IGameState gs, IActionEventArgs cm) {
      _gameState = gs;
      _persistentStorage.Load(this);
    }

    public void Save(IGameDataWriter writer) {
      _randomGenerator.Save(writer);
      ReadOnlyCollection<IEntity> entities = _gameState.GetEntities();
      writer.Write(entities.Count);
      writer.Write(_mainCharacter.ID);
      foreach (IEntity entity in entities) {
        writer.Write(entity.ID);
        entity.Save(writer);
      }
    }

    public void Load(IGameDataReader reader) {
      _randomGenerator.Load(reader);
      Position mapSize = (_gameState.MapWidth, _gameState.MapHeight);
      OnGameEvent(new GameConfiguredEventArgs(mapSize));
      int count = reader.ReadInt();
      int mainCharacterId = reader.ReadInt();
      for (int i = 0; i < count; i++) {
        int entityID = reader.ReadInt();
        IEntity entity = _entityFactory.Get();
        entity.ID = entityID;
        entity.Load(reader);
        Position position = entity.GetComponent<Movement>().EntityPosition;
        _gameState.CreateEntityAtPosition(entity, position);
        OnGameEvent(new EntityCreatedEventArgs(entity));
        OnGameEvent(new EntityMovedEventArgs(entity, position));
        if (entityID == mainCharacterId) {
          OnGameEvent(new MainCharacterChangedEventArgs(entity));
        }

      }
    }

  }
}

