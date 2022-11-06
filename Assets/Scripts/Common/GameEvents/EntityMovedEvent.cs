namespace URFCommon {
    /// <summary>
    /// Notify listeners that an entity has moved.
    /// </summary>
    public class EntityMovedEvent : GameEvent {
        /// <summary>
        /// The entity that moved.
        /// </summary>
        public IEntity Entity { get; }
        /// <summary>
        /// The entity's new position.
        /// </summary>
        public (int, int) Position { get; }
        // todo add an old position
        public override GameEventType EventType => GameEventType.EntityMoved;

        public EntityMovedEvent (IEntity entity, int x, int y) {
            Entity = entity;
            Position = (x, y);
        }
    }
}
