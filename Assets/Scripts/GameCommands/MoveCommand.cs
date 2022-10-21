public class MoveCommand : IGameCommand {
    public (int, int) Direction { get; }
    public GameCommandType CommandType => GameCommandType.Move;

    public MoveCommand (int x, int y) {
        Direction = (x, y);
    }
}