namespace URF.Common.GameEvents {
    public class StartGameCommand : GameEvent {
        public override GameEventType EventType => GameEventType.StartGameCommand;
        public override bool IsCommand => true;
    }
}