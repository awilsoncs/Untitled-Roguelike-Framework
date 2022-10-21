using System;

public class GameErrorEvent : IGameEvent {
    public string Message { get; set; }

    public GameEventType EventType => GameEventType.GameError;

    public GameErrorEvent (string message) {
        Message = message;
    }
}