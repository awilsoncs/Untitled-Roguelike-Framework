namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class EntityEvent : EventArgs, IGameEvent {

    public enum EntityMethod {
      Created,
      Updated,
      Deleted
    }

    public IEntity Entity {
      get;
    }

    public EntityMethod Method {
      get;
    }

    public EntityEvent(IEntity entity, EntityMethod method) {
      this.Entity = entity;
      this.Method = method;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleEntityEvent(this);
    }
  }

  public static class EntityEventExtensions {
    public static EntityEvent WasCreated(
      this IEntity entity
    ) {
      return new EntityEvent(entity, EntityEvent.EntityMethod.Created);
    }

    public static EntityEvent WasUpdated(
      this IEntity entity
    ) {
      return new EntityEvent(entity, EntityEvent.EntityMethod.Updated);
    }

    public static EntityEvent WasDeleted(
      this IEntity entity
    ) {
      return new EntityEvent(entity, EntityEvent.EntityMethod.Deleted);
    }
  }



}
