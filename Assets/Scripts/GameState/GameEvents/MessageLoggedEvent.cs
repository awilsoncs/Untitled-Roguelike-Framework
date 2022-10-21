using System;

public class MessageLoggedEvent : IGameEvent {
    public string Message { get; set; }

    public GameEventType EventType => GameEventType.MessageLoggedEvent;

    public MessageLoggedEvent (string message) {
        Message = message;
    }
}