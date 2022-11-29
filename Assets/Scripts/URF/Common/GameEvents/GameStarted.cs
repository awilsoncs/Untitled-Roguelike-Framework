namespace URF.Common.GameEvents {
  using System;

  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameStarted : EventArgs, IGameEvent {

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGameStarted(this);
    }
  }
}
