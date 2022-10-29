using System;
public partial class GameState : IBuildable {
    
    public void SetMainCharacter(int id) {
        IEntity entity = entitiesById[id];
        gameClient.PostEvent(new MainCharacterChangedEvent(id));
        mainCharacter = entity;
    }

    /// <summary>
    /// Create an entity at a given location using the factory blueprint name.
    /// </summary>
    /// <param name="blueprintName">The name of the Entity type in the factory</param>
    /// <param name="x">The horizontal coordinate at which to create the Entity</param>
    /// <param name="y">The vertical coordinate at which to create the Entity</param>
    /// <returns>A reference to the created Entity</returns>
    public int CreateEntityAtPosition(String blueprintName, int x, int y) {
        // todo abstract entities with no location
        var entity = entityFactory.Get(blueprintName);
        entity.GameState = this;
        entities.Add(entity);
        entitiesById.Add(entity.ID, entity);
        gameClient.PostEvent(new EntityCreatedEvent(entity));
        PlaceEntity(entity.ID, x, y);
        return entity.ID;
    }
}