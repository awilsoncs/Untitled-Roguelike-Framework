namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Notify listeners that an entity's visibility has changed.
  /// </summary>
  public class EntityVisibilityChanged : EventArgs, IGameEvent {

    // todo this could probably be a mass update with multiple entities
    /// <summary>
    /// The entity with a new visibility
    /// </summary>
    public IEntity Entity {
      get;
    }

    /// <summary>
    /// The entity's visibility after the update
    /// </summary>
    public bool NewVisibility {
      get;
    }

    public EntityVisibilityChanged(IEntity entity, bool newVisibility) {
      this.Entity = entity;
      this.NewVisibility = newVisibility;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityVisibilityChanged(this);
    }

    public override string ToString() {
      return $"EntityVisibilityChanged({this.Entity}, {this.NewVisibility})";
    }
  }
}
