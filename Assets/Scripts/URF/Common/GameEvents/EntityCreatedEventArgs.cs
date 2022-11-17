using System;
using URF.Common.Entities;

namespace URF.Common.GameEvents {
  public class EntityCreatedEventArgs : EventArgs, IGameEventArgs {

    public IEntity Entity { get; }
    
    public EntityCreatedEventArgs(IEntity entity) {
      Entity = entity;
    }

    public GameEventType EventType => GameEventType.EntityCreated;

  }
}
