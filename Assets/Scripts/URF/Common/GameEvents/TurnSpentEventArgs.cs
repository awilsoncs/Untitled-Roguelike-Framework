using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  public class TurnSpentEventArgs : EventArgs, IGameEventArgs {

    public IEntity Entity { get; }
    
    public TurnSpentEventArgs(IEntity entity) {
      Entity = entity;
    }

    public GameEventType EventType => GameEventType.SpentTurn;

  }
}