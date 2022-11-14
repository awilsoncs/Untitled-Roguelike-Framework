using System;
using URF.Common.GameEvents;

namespace URF.Common {
  public interface IGameEventChannel {

    public event EventHandler<IGameEventArgs> GameEvent;

  }
}
