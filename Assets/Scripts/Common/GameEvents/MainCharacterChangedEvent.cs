public struct MainCharacterChangedEvent : IGameEvent {
    public IEntity Entity {get;}
    public GameEventType EventType => GameEventType.MainCharacterChanged;

    public MainCharacterChangedEvent (IEntity entity) {
        Entity = entity;
    }
}