using System;

public class EntityCreatedEvent : IGameEvent {
    public int EntityID { get; set; }
    public string Appearance { get; set; }
    public GameEventType EventType => GameEventType.EntityCreated;

    public EntityCreatedEvent (int id, string appearance) {
        EntityID = id;
        Appearance = appearance;
    }
}