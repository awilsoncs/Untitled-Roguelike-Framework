namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  public class TurnSpent : EventArgs, IGameEvent {

    public IEntity Entity {
      get;
    }

    public TurnSpent(IEntity entity) {
      this.Entity = entity;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleTurnSpent(this);
    }
  }
}
