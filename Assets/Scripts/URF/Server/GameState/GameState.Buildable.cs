using System;
using URF.Common;
using URF.Common.Entities;
using URF.Common.GameEvents;


namespace URF.Server.GameState
{
    public partial class GameState : IBuildable
    {
        public void SetMainCharacter(int id)
        {
            IEntity entity = entitiesById[id];
            gameClient.PostEvent(new MainCharacterChangedEvent(entity));
            mainCharacter = entity;
        }

        public int CreateEntityAtPosition(string blueprintName, Position position)
        {
            var entity = entityFactory.Get(blueprintName);
            entity.GameState = this;
            entities.Add(entity);
            entitiesById.Add(entity.ID, entity);
            gameClient.PostEvent(new EntityCreatedEvent(entity));
            PlaceEntity(entity.ID, position);
            return entity.ID;
        }
    }
}