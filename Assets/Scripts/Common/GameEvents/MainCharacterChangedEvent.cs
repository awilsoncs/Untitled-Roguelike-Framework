/// <summary>
/// Notify listeners that the main character has changed.
/// </summary>
public struct MainCharacterChangedEvent : IGameEvent {
    /// <summary>
    /// The new main character
    /// </summary>
    public IEntity Entity {get;}
    // todo add a reference to the old main character here
    public GameEventType EventType => GameEventType.MainCharacterChanged;

    public MainCharacterChangedEvent (IEntity entity) {
        Entity = entity;
    }
}