namespace URF.Common.GameEvents {
    /// <summary>
    /// Notify listeners that the game has experienced an error.
    /// </summary>
    public class GameErrorEvent : GameEvent {
        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; }

        public override GameEventType EventType => GameEventType.GameError;

        public GameErrorEvent (string message) {
            Message = message;
        }
    }
}
