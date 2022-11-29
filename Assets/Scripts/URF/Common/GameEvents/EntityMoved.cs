namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Notify listeners that an entity has moved.
  /// </summary>
  public class EntityMoved : EventArgs, IGameEvent {

    /// <summary>
    /// The entity that moved.
    /// </summary>
    public IEntity Entity {
      get;
    }

    /// <summary>
    /// The entity's new position.
    /// </summary>
    public Position Position {
      get;
    }

    public EntityMoved(IEntity entity, Position position) {
      this.Entity = entity;
      this.Position = position;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityMoved(this);
    }
  }
}
