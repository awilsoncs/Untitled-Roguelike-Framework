namespace URFCommon {
    /// <summary>
    /// Provide a pluggable system for updating a client to a GameState.
    /// </summary>
    public interface IGameClient {
        // todo rename this IGameEventListener
        void PostEvent(IGameEvent ev);
    }
}
