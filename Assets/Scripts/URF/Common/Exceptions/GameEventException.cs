
using System;
using URF.Common.GameEvents;
/// <summary>
/// Thrown by any system when it receives an event that should not be allowed to happen.
/// </summary>
public class GameEventException : Exception {

  // the offending game event
  public IGameEvent GameEvent {
    get;
  }

  public GameEventException(IGameEvent ev, string message) : base(message) {
    this.GameEvent = ev;
  }
}
