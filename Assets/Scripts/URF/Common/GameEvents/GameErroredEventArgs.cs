using System;

namespace URF.Common.GameEvents {
    /// <summary>
    /// Notify listeners that the game has experienced an error.
    /// </summary>
    public class GameErroredEventArgs : EventArgs, IGameEventArgs {
        /// <summary>
        /// The error message
        /// </summary>
        public string Message { get; }

        public GameEventType EventType => GameEventType.GameError;

        public GameErroredEventArgs (string message) {
            Message = message;
        }
    }
}
