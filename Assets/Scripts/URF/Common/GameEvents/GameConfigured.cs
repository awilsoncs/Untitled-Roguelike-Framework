namespace URF.Common.GameEvents {
  using System;
  using URF.Common.GameState;

  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameConfigured : EventArgs, IGameEvent {

    public IReadOnlyGameState<IReadOnlyCell> GameState {
      get;
    }

    public GameConfigured(IReadOnlyGameState<IReadOnlyCell> gameState) {
      this.GameState = gameState;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGameConfigured(this);
    }
  }
}
