using System;
using URF.Common.GameEvents;

namespace URF.Common {
  public interface IGameEventChannel {

    /// <summary>
    /// Message containing declarative game events.
    /// </summary>
    public event EventHandler<IGameEventArgs> GameEvent;

  }
}
