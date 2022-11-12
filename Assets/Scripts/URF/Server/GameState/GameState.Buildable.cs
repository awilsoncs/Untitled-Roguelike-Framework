using System;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;

namespace URF.Server.GameState {
  public partial class GameState {

    public void SetMainCharacter(int id) {
      IEntity entity = _entitiesById[id];
      _gameClient.PostEvent(new MainCharacterChangedEvent(entity));
      _mainCharacter = entity;
    }

    public int CreateEntityAtPosition(string blueprintName, Position position) {
      Entity entity = _entityFactory.Get(blueprintName);
      entity.GameState = this;
      _entities.Add(entity);
      _entitiesById.Add(entity.ID, entity);
      _gameClient.PostEvent(new EntityCreatedEvent(entity));
      PlaceEntity(entity.ID, position);
      return entity.ID;
    }

  }
}
