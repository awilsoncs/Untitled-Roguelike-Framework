using URF.Common.Entities;

namespace URF.Common.GameEvents {
  public class EntityCreatedEvent : GameEvent {

    public IEntity Entity { get; }

    public override GameEventType EventType => GameEventType.EntityCreated;

    public EntityCreatedEvent(IEntity entity) {
      Entity = entity;
    }

  }
}
