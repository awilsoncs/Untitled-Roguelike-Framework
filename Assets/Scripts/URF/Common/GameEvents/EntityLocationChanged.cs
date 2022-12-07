namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Notify listeners that an entity has changed location, including moving around the board,
  /// or appearing and disappearing from the map. This event should not be taken to mean that the
  /// entity has simply become invisible. It should only be emitted when the entity physically
  /// moves.
  /// </summary>
  public class EntityLocationChanged : EventArgs, IGameEvent {

    /// <summary>
    /// Represents the possible ways that an entity's location can change.
    /// </summary>
    public enum EventSubType {
      // In this subtype, the entity was placed onto the map from off the map.
      Placed,
      // In this subtype, the entity was removed from the map.
      Removed,
      // In this subtype, the entity moved from one position on the map to another position on the
      // map.
      Moved
    }

    /// <summary>
    /// The entity that changed locations. Cannot be null.
    /// </summary>
    public IEntity Entity {
      get;
    }

    /// <summary>
    /// The type of location change that occurred.
    /// </summary>
    /// <value></value>
    public EventSubType SubType {
      get;
    }

    /// <summary>
    /// If the entity was removed or moved, the entity's old position. Otherwise, (-1, -1).
    /// </summary>
    public Position OldPosition {
      get;
    }

    /// <summary>
    /// If the entity was placed or moved, the entity's new position. Otherwise, (-1, -1).
    /// </summary>
    /// <value></value>
    public Position NewPosition {
      get;
    }

    /// <summary>
    /// Raw constructor for the EntityMoved. Client code should prefer static constructors.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="subType"></param>
    /// <param name="oldPosition"></param>
    /// <param name="newPosition"></param>
    public EntityLocationChanged(
      IEntity entity,
      EventSubType subType,
      Position oldPosition,
      Position newPosition
    ) {
      if (subType is EventSubType.Moved or EventSubType.Placed) {
        if (newPosition.X < 0 || newPosition.Y < 0) {
          throw new ArgumentException(
            string.Join(
              "New position coordinates must be positive when entity is moved or placed ",
              $"{newPosition}"
            )
          );
        }
      }

      if (subType is EventSubType.Moved or EventSubType.Removed) {
        if (oldPosition.X < 0 || oldPosition.Y < 0) {
          throw new ArgumentException(
            string.Join(
              "New position coordinates must be positive when entity is moved or removed ",
              $"{newPosition}"
            )
          );
        }
      }

      if (subType == EventSubType.Placed) {
        if (oldPosition.X != -1 || oldPosition.Y != -1) {
          throw new ArgumentException(
            "Old position coordinates must be -1 when an entity is placed.");
        }
      } else if (subType is EventSubType.Removed) {
        if (newPosition.X != -1 || newPosition.Y != -1) {
          throw new ArgumentException(
            "New position coordinates must be -1 when an entity is removed.");
        }
      }

      this.Entity = entity ?? throw new ArgumentNullException("Entity must not be null");
      this.SubType = subType;
      this.OldPosition = oldPosition;
      this.NewPosition = newPosition;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityLocationChanged(this);
    }

    /// <summary>
    /// Create a variant of the EntityLocationChanged event representing an entity moving within the
    /// map.
    /// </summary>
    /// <param name="entity">The entity that is moving</param>
    /// <param name="oldPosition">The entity's old Position</param>
    /// <param name="newPosition">The entity's new Position</param>
    public static EntityLocationChanged EntityMoved(
      IEntity entity,
      Position oldPosition,
      Position newPosition
    ) {
      // note that the constructor will throw an ArgumentException if the old or new position have
      // negative coordinates
      return new EntityLocationChanged(
        entity,
        EventSubType.Moved,
        oldPosition,
        newPosition
      );
    }

    /// <summary>
    /// Create an EntityChangedLocation event of subtype Placed, indicating an existing entity
    /// has been placed onto the map.
    /// </summary>
    /// <param name="entity">The entity that is being placed</param>
    /// <param name="newPosition">The entity's new Position</param>
    public static EntityLocationChanged EntityPlaced(IEntity entity, Position newPosition) {
      // note that the constructor will throw an ArgumentException if the new position has negative
      // coordinates
      return new EntityLocationChanged(
        entity,
        EventSubType.Placed,
        new Position(-1, -1),
        newPosition
      );
    }

    /// <summary>
    /// Create an EntityChangedLocation event of subtype Removed, indicating an entity has been
    /// removed from the map but not deleted.
    /// </summary>
    /// <param name="entity">The entity that is being removed</param>
    /// <param name="oldPosition">The entity's old Position</param>
    public static EntityLocationChanged EntityRemoved(IEntity entity, Position oldPosition) {
      // note that the constructor will throw an ArgumentException if the old position has negative
      // coordinates
      return new EntityLocationChanged(
        entity,
        EventSubType.Removed,
        oldPosition,
        new Position(-1, -1)
      );
    }
  }
}
