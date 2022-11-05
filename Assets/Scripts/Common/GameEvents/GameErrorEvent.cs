using System;

/// <summary>
/// Notify listeners that the game has experienced an error.
/// </summary>
public struct GameErrorEvent : IGameEvent {
    /// <summary>
    /// The error message
    /// </summary>
    public string Message { get; }

    public GameEventType EventType => GameEventType.GameError;

    public GameErrorEvent (string message) {
        Message = message;
    }
}