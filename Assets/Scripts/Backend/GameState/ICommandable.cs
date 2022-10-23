/// <summary>
/// Provide an interface for a gameplay client to send commands to the
/// game state.
/// </summary>
public interface ICommandable {
    void PushCommand(IGameCommand cm);
}