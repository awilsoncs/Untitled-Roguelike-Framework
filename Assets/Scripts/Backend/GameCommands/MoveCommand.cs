public struct MoveCommand : IGameCommand {
    public int EntityId {get;}
    public (int, int) Direction { get; }
    public GameCommandType CommandType => GameCommandType.Move;

    public MoveCommand (int id, int x, int y) {
        EntityId = id;
        Direction = (x, y);
    }
}