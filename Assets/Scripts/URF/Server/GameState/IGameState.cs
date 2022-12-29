namespace URF.Server.GameState {
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.GameState;

  /// <summary>
  /// Provide a repository interface for storing entities. Handles entities and their place within
  /// the game world. Beyond position, the IGameState does not have any awareness of entities'
  /// information.
  /// </summary>
  public interface IGameState : IReadOnlyGameState {

    /// <summary>
    /// Persist an entity in the game state. This method should be called when client code has
    /// created a new entity, so that other systems can access that entity.
    /// /// </summary>
    /// <param name="entity">The entity to be persisted.</param>
    void CreateEntity(IEntity entity);

    /// <summary>
    /// Places an IEntity that does not currently appear on the map at the given position.
    /// </summary>
    /// <param name="entity">The IEntity to be placed</param>
    /// <param name="position">The Position at which to place the IEntity</param>
    void PlaceEntityOnMap(IEntity entity, Position position);

    /// <summary>
    /// Remove an entity from the physical map, but leave it alive.
    /// </summary>
    /// <param name="entity">The IEntity to remove.</param>
    void RemoveEntityFromMap(IEntity entity);

    /// <summary>
    /// Move an entity from one position on a map to another position on the map.
    /// </summary>
    /// <param name="entity">The IEntity to move.</param>
    /// <param name="from">The Position the IEntity is currently at.</param>
    /// <param name="to">The Position the IEntity should be moved to.</param>
    /// <raises>
    void MoveEntity(IEntity entity, Position to);

    /// <summary>
    /// Delete an entity from the GameState. Does not guarantee that references to the entity are
    /// removed.
    /// </summary>
    /// <param name="entity">The IEntity to delete.</param>
    void DeleteEntity(IEntity entity);

    new Cell GetCell(Position position);
  }
}
