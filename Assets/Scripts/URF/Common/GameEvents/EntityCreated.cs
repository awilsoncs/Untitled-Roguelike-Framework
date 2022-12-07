namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class EntityCreated : EventArgs, IGameEvent {

    public IEntity Entity {
      get;
    }

    public EntityCreated(IEntity entity) {
      this.Entity = entity;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityCreated(this);
    }
  }
}
