public class EntityVisibilityChangedEvent : IGameEvent {
    public int EntityID { get; set; }
    public bool NewVisibility { get; set; }
    public GameEventType EventType => GameEventType.EntityVisibilityChanged;

    public EntityVisibilityChangedEvent (int id, bool newVisibility) {
        EntityID = id;
        NewVisibility = newVisibility;
    }
}