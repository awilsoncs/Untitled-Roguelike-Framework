using System;

public class MessageLoggedEvent : IGameEvent {
    public string Message { get; set; }

    public GameEventType EventType => GameEventType.MessageLogged;

    public MessageLoggedEvent (string message) {
        Message = message;
    }
}