public struct GameStartedEvent : IGameEvent {
    public GameEventType EventType => GameEventType.GameStarted;
}