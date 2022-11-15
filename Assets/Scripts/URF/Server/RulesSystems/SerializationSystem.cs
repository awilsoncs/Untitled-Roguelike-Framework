using System.Collections.Generic;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class SerializationSystem : BaseRulesSystem, IPersistableObject {

    private const int saveVersion = 1;

    private PersistentStorage _persistentStorage;

    private IEntityFactory _entityFactory;

    private IEntity _mainCharacter;

    private IGameState _gameState;

    public override void ApplyPlugins(PluginBundle pluginBundle) {
      _persistentStorage = pluginBundle.PersistentStorage;
      _entityFactory = pluginBundle.EntityFactory;
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

    public void Save(GameDataWriter writer) {
      List<IEntity> entities = _gameState.GetEntities();
      writer.Write(entities.Count);
      writer.Write(_mainCharacter.ID);
      for(int i = 0; i < entities.Count; i++) {
        writer.Write(entities[i].ID);
        entities[i].Save(writer);
      }
    }

    public void Load(GameDataReader reader) {
      Position mapSize = (_gameState.MapWidth, _gameState.MapHeight);
      OnGameEvent(new GameConfiguredEventArgs(mapSize));
      int count = reader.ReadInt();
      int mainCharacterId = reader.ReadInt();
      for(int i = 0; i < count; i++) {
        int entityID = reader.ReadInt();
        Entity entity = _entityFactory.Get();
        entity.ID = entityID;
        entity.Load(reader);
        Position position = entity.GetComponent<Movement>().EntityPosition;
        _gameState.CreateEntityAtPosition(entity, position);
        OnGameEvent(new EntityCreatedEventArgs(entity));
        OnGameEvent(new EntityMovedEventArgs(entity, position));
        if(entityID == mainCharacterId) {
          OnGameEvent(new MainCharacterChangedEventArgs(entity));
        }
        
      }
    }

  }
}

