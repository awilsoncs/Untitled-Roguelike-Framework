/// <summary>
/// A command for sending client actions to the game state.
/// </summary>
public interface IGameCommand {
    GameCommandType CommandType {get;}
}