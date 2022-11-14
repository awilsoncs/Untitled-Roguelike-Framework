using System;

namespace URF.Common.GameEvents {
  using PositionDelta = Position;

  public class LoadActionEventArgs : EventArgs, IActionEventArgs {

    public GameEventType EventType => GameEventType.Load;

  }
}
