
namespace URF.Common.Exceptions {
  using System;
  using System.Runtime.Serialization;
  using URF.Common.GameEvents;

  /// <summary>
  /// Thrown by any system when it receives an event that should not be allowed to happen.
  /// </summary>
  [Serializable]
  public class GameEventException : Exception {

    // the offending game event
    public IGameEvent GameEvent {
      get;
    }

    public GameEventException(IGameEvent ev, string message) : base(message) {
      this.GameEvent = ev;
    }

    public GameEventException() : base() { }
    public GameEventException(string message) : base(message) { }
    public GameEventException(string message, Exception exception)
      : base(message, exception) { }
    protected GameEventException(SerializationInfo info, StreamingContext context)
      : base(info, context) { }
  }
}

