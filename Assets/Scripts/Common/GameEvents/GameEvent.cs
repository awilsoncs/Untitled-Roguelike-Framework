namespace URFCommon {
    public abstract class GameEvent : IGameEvent {
        public abstract GameEventType EventType {get;}
        public virtual bool IsCommand => false;
    }
}
