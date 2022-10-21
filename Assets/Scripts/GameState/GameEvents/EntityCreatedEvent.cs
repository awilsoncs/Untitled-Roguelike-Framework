using System;

public class EntityCreatedEvent : IGameEvent {
    public int EntityID { get; set; }
    public string Appearance { get; set; }
    public (int, int) Position { get; set; }

    public GameEventType EventType => GameEventType.EntityCreatedEvent;

    public EntityCreatedEvent (int id, string appearance, int x, int y) {
        EntityID = id;
        Appearance = appearance;
        Position = (x, y);
    }
}