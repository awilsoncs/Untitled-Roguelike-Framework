namespace URF.Server.GameState {
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using URF.Common;
  using URF.Common.Entities;
  using URF.Common.Exceptions;
  using URF.Common.GameEvents;

  /// <summary>
  /// Represent the state of the game at the level of entities and their positions on the map.
  /// </summary>
  public class GameState : BaseGameEventChannel, IGameState {

    /// <summary>
    /// A position representing the boundary corner of the map.
    /// </summary>
    /// <value>A nonnegative position</value>
    public Position MapSize {
      get;
    }

    private readonly Cell[,] map;

    // Keep track of all entities.
    private readonly HashSet<IEntity> uniqueEntities = new();

    // Maintain an index of where each mapped entity is so that we can find them easily. If an
    // entity does not appear as a key in this dictionary, it is not on the map.
    private readonly Dictionary<IEntity, Position> positionsByEntity = new();

    private readonly Dictionary<int, IEntity> entitiesById = new();

    /// <summary>
    /// Create a GameState with map dimensions equal to the given parameters.
    /// </summary>
    /// <param name="mapWidth">The positive integer width of the map</param>
    /// <param name="mapHeight">The positive integer height of the map</param>
    /// <exception cref=ArgumentException>
    /// Throws ArgumentException when either parameter is not positive.
    /// </exception>
    public GameState(int mapWidth, int mapHeight) {
      if (mapWidth < 1 || mapHeight < 1) {
        throw new ArgumentException(
          $"Map dimensions must be positive, received ({mapWidth}, {mapHeight}).");
      }

      this.MapSize = new Position(mapWidth, mapHeight);
      this.map = new Cell[this.MapSize.X, this.MapSize.Y];
      this.map.Populate(() => new Cell());
    }

    /// <summary>
    /// Creates a new IEntity and emits an EntityCreated event containing the entity.
    /// </summary>
    /// <exception cref=ArgumentNullException>
    /// Throws ArgumentNullException when entity is null.
    /// </exception>
    /// <exception cref=ArgumentException>
    /// Throws ArgumentException when the entity already exists in the game state.
    /// </exception>
    public void CreateEntity(IEntity entity) {
      if (entity == null) {
        throw new ArgumentNullException("Cannot create a null entity");
      } else if (this.uniqueEntities.Contains(entity)) {
        throw new ArgumentException("Cannot create an entity that already exists");
      }

      this.entitiesById[entity.ID] = entity;
      _ = this.uniqueEntities.Add(entity);
      this.OnGameEvent(entity.WasCreated());
    }

    /// <summary>
    /// Get a readonly list of all entities in the game state, including those that are not on the
    /// map. The order of the entities is not guaranteed.
    /// </summary>
    /// <returns>A read-only collection of entities</returns>
    public IReadOnlyCollection<IEntity> GetAllEntities() {
      return this.uniqueEntities.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public IEntity GetEntityById(int id) {
      return this.entitiesById[id];
    }

    /// <summary>
    /// Places an entity on the map and emits an EntityLocationChanged.Placed event.
    /// </summary>
    /// <exception cref=ArgumentNullException>
    /// Throws ArgumentNullException when entity is null.
    /// </exception>
    /// <exception cref=ArgumentException>
    /// Throws ArgumentException when the position is outside of the map bounds, the entity is
    /// already on the map.
    /// </exception>
    /// <exception cref=EntityDetachedException>
    /// Thrown if entity has not been created.
    /// </exception>
    public void PlaceEntityOnMap(IEntity entity, Position position) {
      if (entity == null) {
        throw new ArgumentNullException("Cannot place a null entity");
      } else if (!this.IsInBounds(position)) {
        throw new ArgumentException($"Position must be in bounds: {position} vs {this.MapSize}");
      } else if (this.positionsByEntity.ContainsKey(entity)) {
        Position currentPosition = this.positionsByEntity[entity];
        throw new ArgumentException(
          $"Entity {entity} is on the map at position {currentPosition}. Use MoveEntity instead."
        );
      } else if (!this.uniqueEntities.Contains(entity)) {
        throw new EntityDetachedException(
          $"A detached entity cannot be placed on the map.");
      }

      this.PlaceEntity(entity, position);
      this.OnGameEvent(EntityLocationChanged.EntityPlaced(entity, position));
    }

    /// <summary>
    /// Remove an entity from the map and emit an EntityLocationChanged.Removed event.
    /// </summary>
    /// <param name="entity">The entity to remove from the map</param>
    /// <exception cref=ArgumentNullException>
    /// Throws ArgumentNullException when entity is null.
    /// </exception>
    /// <exception cref=ArgumentException>
    /// Throws ArgumentException when the entity does not appear on the map.
    /// </exception>
    public void RemoveEntityFromMap(IEntity entity) {
      if (entity == null) {
        throw new ArgumentNullException("Cannot remove a null entity from the map.");
      } else if (!this.uniqueEntities.Contains(entity)) {
        throw new EntityDetachedException("Entity must be created before it can be removed.");
      } else if (!this.positionsByEntity.ContainsKey(entity)) {
        throw new ArgumentException($"Entity {entity} is not on the map.");
      }

      Position oldPosition = this.positionsByEntity[entity];
      _ = this.positionsByEntity.Remove(entity);
      this.map[oldPosition.X, oldPosition.Y].RemoveEntity(entity);
      this.OnGameEvent(EntityLocationChanged.EntityRemoved(entity, oldPosition));
    }

    /// <summary>
    /// Move an entity that's currently on the map from one position to a new one, emitting a
    /// EntityLocationChanged.Moved event containing the entity and both old and new positions.
    /// </summary>
    /// <param name="entity">The IEntity to be moved</param>
    /// <param name="to">The Position to move the entity to</param>
    /// <exception cref=ArgumentNullException>When entity is null</exception>
    /// <exception cref=ArgumentException>
    /// When the entity is not on the map or the position is not in bounds.
    /// </exception>
    public void MoveEntity(IEntity entity, Position to) {
      if (entity == null) {
        throw new ArgumentNullException("Cannot remove a null entity from the map.");
      } else if (!this.uniqueEntities.Contains(entity)) {
        throw new EntityDetachedException("Entity must be created before it can be moved.");
      } else if (!this.positionsByEntity.ContainsKey(entity)) {
        throw new ArgumentException($"Entity {entity} is not on the map.");
      } else if (!this.IsInBounds(to)) {
        throw new ArgumentException($"Position must be in bounds: {to} vs {this.MapSize}");
      }

      Position oldPosition = this.positionsByEntity[entity];
      Cell origin = this.GetCell(oldPosition);
      origin.RemoveEntity(entity);
      this.PlaceEntity(entity, to);
      this.OnGameEvent(EntityLocationChanged.EntityMoved(entity, oldPosition, to));
    }

    /// <summary>
    /// Delete an entity from the GameState, emitting an EntityDeleted event.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <exception cref=ArgumentNullException>When entity is null</exception>
    /// <exception cref=ArgumentException>When entity does not exist in the game state</exception>
    public void DeleteEntity(IEntity entity) {
      if (entity == null) {
        throw new ArgumentNullException("Entity to delete must not be null.");
      } else if (!this.uniqueEntities.Contains(entity)) {
        throw new ArgumentException($"{entity} does not exist in the game state.");
      }

      if (this.positionsByEntity.ContainsKey(entity)) {
        this.RemoveEntityFromMap(entity);
      }
      _ = this.entitiesById.Remove(entity.ID);
      _ = this.uniqueEntities.Remove(entity);
      this.OnGameEvent(entity.WasDeleted());
    }

    /// <summary>
    /// Get one cell of the map.
    /// </summary>
    /// <param name="position">The Position of the cell to retrieve</param>
    /// <returns>A Cell at the given position</returns>
    /// <exception cref=ArgumentException>When the position is not in bounds.</exception>
    public Cell GetCell(Position position) {
      if (!this.IsInBounds(position)) {
        throw new ArgumentException(
          $"Cannot get out-of bounds cell: {position} not in {this.MapSize}");
      }
      (int x, int y) = position;
      return this.map[x, y];
    }

    /// <summary>
    /// Get the location of an entity on the map. If the entity is not on the map, return an invalid
    /// position.
    /// </summary>
    /// <param name="entity">The entity to locate</param>
    /// <returns>
    /// A Position where the entity was found, or Position.Invalid if it's not on the map
    /// </returns>
    /// <exception cref=ArgumentNullException>When entity is null</exception>
    /// <exception cref=EntityDetachedException>
    /// When the entity has not been created in the game state.
    /// </exception>
    public Position LocateEntityOnMap(IEntity entity) {
      if (entity == null) {
        throw new ArgumentNullException("Cannot locate a null entity.");
      } else if (!this.uniqueEntities.Contains(entity)) {
        throw new EntityDetachedException("Entity must be created before it can be located.");
      }

      if (this.positionsByEntity.ContainsKey(entity)) {
        return this.positionsByEntity[entity];
      }

      return Position.Invalid;
    }

    private bool IsInBounds(Position p) {
      return 0 <= p.X && p.X < this.MapSize.X && 0 <= p.Y && p.Y < this.MapSize.Y;
    }

    private void PlaceEntity(IEntity entity, Position p) {
      Cell destination = this.GetCell(p);
      destination.PutContents(entity);
      this.positionsByEntity[entity] = p;
    }

  }
}
