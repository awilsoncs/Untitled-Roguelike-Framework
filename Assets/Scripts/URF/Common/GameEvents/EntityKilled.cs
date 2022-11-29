namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  // todo do we need to separate this from entity death?
  /// <summary>
  /// Notify listeners that an entity has been removed from the game.
  /// </summary>
  public class EntityKilled : EventArgs, IGameEvent {

    /// <summary>
    /// The entity that was removed.
    /// </summary>
    public IEntity Entity {
      get;
    }

    public EntityKilled(IEntity entity) {
      this.Entity = entity;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityKilled(this);
    }
  }
}
