/// <summary>
/// Provide a pluggable system for updating a client to a GameState.
/// </summary>
public interface IGameClient {
    void PostEvent(IGameEvent ev);
}