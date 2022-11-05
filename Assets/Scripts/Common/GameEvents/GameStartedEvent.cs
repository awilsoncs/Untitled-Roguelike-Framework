/// <summary>
/// Notify listeners that a new game has begun.
/// </summary>
public struct GameStartedEvent : IGameEvent {
    public GameEventType EventType => GameEventType.GameStarted;
}