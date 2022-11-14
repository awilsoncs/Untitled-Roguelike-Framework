using System;

namespace URF.Common.GameEvents {
  using PositionDelta = Position;

  public class MoveActionEventArgs : EventArgs, IActionEventArgs {

    public int EntityId { get; }

    public PositionDelta Direction { get; }

    public GameEventType EventType => GameEventType.MoveCommand;

    public bool IsCommand => true;

    public MoveActionEventArgs(int id, PositionDelta delta) {
      EntityId = id;
      Direction = delta;
    }

  }
}
