using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  // todo do we need to separate this from entity death?
  /// <summary>
  /// Notify listeners that an entity has been removed from the game.
  /// </summary>
  public class EntityKilledEventArgs : EventArgs, IGameEventArgs {

    /// <summary>
    /// The entity that was removed.
    /// </summary>
    public IEntity Entity { get; }

    public GameEventType EventType => GameEventType.EntityKilled;

    public EntityKilledEventArgs(IEntity entity) {
      Entity = entity;
    }

  }
}
