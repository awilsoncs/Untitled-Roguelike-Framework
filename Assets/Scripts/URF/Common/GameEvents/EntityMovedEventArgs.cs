using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that an entity has moved.
  /// </summary>
  public class EntityMovedEventArgs : EventArgs, IGameEventArgs {

    /// <summary>
    /// The entity that moved.
    /// </summary>
    public IEntity Entity { get; }

    /// <summary>
    /// The entity's new position.
    /// </summary>
    public Position Position { get; }

    // todo add an old position
    public GameEventType EventType => GameEventType.EntityMoved;

    public EntityMovedEventArgs(IEntity entity, Position position) {
      Entity = entity;
      Position = position;
    }

  }
}
