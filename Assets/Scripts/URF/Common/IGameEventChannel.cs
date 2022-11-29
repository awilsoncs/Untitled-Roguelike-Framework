namespace URF.Common {
  using System;
  using URF.Common.GameEvents;

  public interface IGameEventChannel : IEventHandler {

    /// <summary>
    /// Message containing declarative game events.
    /// </summary>
    event EventHandler<IGameEvent> GameEvent;

    void Connect(IGameEventChannel eventChannel);

    void HandleEvent(object sender, IGameEvent ev);

  }
}
