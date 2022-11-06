namespace URFCommon {
    public class MoveCommand : GameEvent {
        public int EntityId {get;}
        public (int, int) Direction { get; }
        public override GameEventType EventType => GameEventType.MoveCommand;
        public override bool IsCommand => true;

        public MoveCommand (int id, int x, int y) {
            EntityId = id;
            Direction = (x, y);
        }
    }
}
