public class EntityMovedEvent : IGameEvent {
    public int EntityID { get; set; }
    public (int, int) Position { get; set; }
    public GameEventType EventType => GameEventType.EntityMoved;

    public EntityMovedEvent (int id, int x, int y) {
        EntityID = id;
        Position = (x, y);
    }
}