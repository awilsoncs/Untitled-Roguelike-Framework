namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Notify listeners that an entity is attempting to get an item.
  /// </summary>
  public class GetAction : EventArgs, IGameEvent {

    /// <summary>
    /// The entity that moved.
    /// </summary>
    public IEntity Entity {
      get;
    }

    /// <summary>
    /// The entity's new position.
    /// </summary>
    public IEntity Target {
      get;
    }

    public GetAction(IEntity entity, IEntity target) {
      this.Entity = entity;
      this.Target = target;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGetAction(this);
    }
  }
}
