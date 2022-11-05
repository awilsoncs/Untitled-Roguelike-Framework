/// <summary>
/// Notify listeners that an entity has moved.
/// </summary>
public struct EntityMovedEvent : IGameEvent {
    /// <summary>
    /// The entity that moved.
    /// </summary>
    public IEntity Entity { get; }
    /// <summary>
    /// The entity's new position.
    /// </summary>
    public (int, int) Position { get; set; }
    // todo add an old position
    public GameEventType EventType => GameEventType.EntityMoved;

    public EntityMovedEvent (IEntity entity, int x, int y) {
        Entity = entity;
        Position = (x, y);
    }
}