namespace URF.Effects {
  using URF.Common;
  using URF.Server.GameState;

  /// <summary>
  /// Represent a change to the game world.
  /// </summary>
  public interface IEffect {

    /// <summary>
    /// Apply the effect to the game world.
    /// </summary>
    /// <param name="forwarder">Port for pushing events into</param>
    /// <param name="gameState">The game state</param>
    void Apply(IEventForwarder forwarder, IGameState gameState);
  }
}
