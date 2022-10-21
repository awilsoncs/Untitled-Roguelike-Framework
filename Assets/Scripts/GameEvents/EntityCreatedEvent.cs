using System;

public class EntityCreatedEvent : IGameEvent {
    public IEntity Entity { get; set; }
    public GameEventType EventType => GameEventType.EntityCreated;

    public EntityCreatedEvent (IEntity entity) {
        Entity = entity;
    }
}