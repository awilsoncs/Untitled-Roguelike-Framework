using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that an entity's visibility has changed.
  /// </summary>
  public class EntityVisibilityChangedEventArgs : EventArgs, IGameEventArgs {

    // todo this could probably be a mass update with multiple entities
    /// <summary>
    /// The entity with a new visibility
    /// </summary>
    public IEntity Entity { get; }

    /// <summary>
    /// The entity's visibility after the update
    /// </summary>
    public bool NewVisibility { get; }

    public GameEventType EventType => GameEventType.EntityVisibilityChanged;

    public EntityVisibilityChangedEventArgs(IEntity entity, bool newVisibility) {
      Entity = entity;
      NewVisibility = newVisibility;
    }

  }
}
