using System;

namespace URFCommon {
    // todo do we need to separate this from entity death?
    /// <summary>
    /// Notify listeners that an entity has been removed from the game.
    /// </summary>
    public class EntityKilledEvent : GameEvent {
        /// <summary>
        /// The entity that was removed.
        /// </summary>
        public IEntity Entity { get; }

        public override GameEventType EventType => GameEventType.EntityKilled;

        public EntityKilledEvent (IEntity entity) {
            Entity = entity;
        }
    }
}
