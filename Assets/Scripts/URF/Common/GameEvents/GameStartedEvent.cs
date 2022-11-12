namespace URF.Common.GameEvents {
    /// <summary>
    /// Notify listeners that a new game has begun.
    /// </summary>
    public class GameStartedEvent : GameEvent {
        public override GameEventType EventType => GameEventType.GameStarted;
    }
}