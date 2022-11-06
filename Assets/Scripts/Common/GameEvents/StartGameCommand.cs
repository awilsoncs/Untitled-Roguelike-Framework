namespace URFCommon {
    public class StartGameCommand : GameEvent {
        public override GameEventType EventType => GameEventType.StartGameCommand;
        public override bool IsCommand => true;
    }
}