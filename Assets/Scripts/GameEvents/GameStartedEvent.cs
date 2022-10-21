public class GameStartedEvent : IGameEvent {
    public GameEventType EventType => GameEventType.GameStarted;
}