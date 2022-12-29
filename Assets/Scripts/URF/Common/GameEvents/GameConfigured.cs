namespace URF.Common.GameEvents {
  using System;
  using URF.Common.GameState;

  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameConfigured : EventArgs, IGameEvent {

    public IReadOnlyGameState GameState {
      get;
    }

    public GameConfigured(IReadOnlyGameState gameState) {
      this.GameState = gameState;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGameConfigured(this);
    }
  }
}
