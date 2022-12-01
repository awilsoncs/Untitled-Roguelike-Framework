namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;
  using PositionDelta = Position;

  public class MoveAction : EventArgs, IGameEvent {

    public IEntity Entity {
      get;
    }

    public PositionDelta Direction {
      get;
    }

    public bool IsCommand => true;

    public MoveAction(IEntity entity, PositionDelta delta) {
      this.Entity = entity;
      this.Direction = delta;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleMoveAction(this);
    }
  }
}
