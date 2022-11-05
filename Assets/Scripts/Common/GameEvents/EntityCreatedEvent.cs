using System;

public struct EntityCreatedEvent : IGameEvent {
    public IEntity Entity { get; }
    public GameEventType EventType => GameEventType.EntityCreated;

    public EntityCreatedEvent (IEntity entity) {
        Entity = entity;
    }
}