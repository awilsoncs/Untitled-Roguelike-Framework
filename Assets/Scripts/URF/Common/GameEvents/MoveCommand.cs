namespace URF.Common.GameEvents {
  using PositionDelta = Position;

  public class MoveCommand : GameEvent {

    public int EntityId { get; }

    public PositionDelta Direction { get; }

    public override GameEventType EventType => GameEventType.MoveCommand;

    public override bool IsCommand => true;

    public MoveCommand(int id, PositionDelta delta) {
      EntityId = id;
      Direction = delta;
    }

  }
}
