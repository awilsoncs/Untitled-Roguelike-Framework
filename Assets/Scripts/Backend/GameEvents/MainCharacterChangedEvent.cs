public class MainCharacterChangedEvent : IGameEvent {
    public int Id { get; set; }
    public GameEventType EventType => GameEventType.MainCharacterChanged;

    public MainCharacterChangedEvent (int id) {
        Id = id;
    }
}