namespace URF.Common.GameEvents {
  using System;
  using PositionDelta = Position;

  public class MoveAction : EventArgs, IGameEvent {

    public int EntityId {
      get;
    }

    public PositionDelta Direction {
      get;
    }

    public bool IsCommand => true;

    public MoveAction(int id, PositionDelta delta) {
      this.EntityId = id;
      this.Direction = delta;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleMoveAction(this);
    }
  }
}
