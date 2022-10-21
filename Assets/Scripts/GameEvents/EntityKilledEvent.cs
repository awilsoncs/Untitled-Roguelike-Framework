using System;

public class EntityKilledEvent : IGameEvent {
    public int EntityId { get; set; }

    public GameEventType EventType => GameEventType.EntityKilled;

    public EntityKilledEvent (int id) {
        EntityId = id;
    }
}