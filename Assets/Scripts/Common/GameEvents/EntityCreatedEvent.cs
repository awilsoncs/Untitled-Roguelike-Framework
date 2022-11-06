using System;

namespace URFCommon {
    public class EntityCreatedEvent : GameEvent {
        public IEntity Entity { get; }
        public override GameEventType EventType => GameEventType.EntityCreated;
        
        public EntityCreatedEvent (IEntity entity) {
            Entity = entity;
        }
    }
}
