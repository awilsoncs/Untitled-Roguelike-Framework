namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class EntityCreated : EventArgs, IGameEvent {

    public IEntity Entity {
      get;
    }

    public Position Position {
      get;
    }

    public EntityCreated(IEntity entity, Position position) {
      this.Entity = entity;
      this.Position = position;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityCreated(this);
    }
  }
}
