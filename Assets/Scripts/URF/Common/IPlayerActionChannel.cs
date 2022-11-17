using System;
using URF.Common.GameEvents;

namespace URF.Common {
  public interface IPlayerActionChannel {

    public event EventHandler<IActionEventArgs> PlayerAction;

  }
}
