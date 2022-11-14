using System;

namespace URF.Common.GameEvents {
  using PositionDelta = Position;

  public class SaveActionEventArgs : EventArgs, IActionEventArgs {

    public GameEventType EventType => GameEventType.Save;

  }
}
