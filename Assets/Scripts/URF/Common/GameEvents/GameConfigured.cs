namespace URF.Common.GameEvents {
  using System;

  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameConfigured : EventArgs, IGameEvent {

    public Position MapSize {
      get;
    }

    public GameConfigured(Position mapSize) {
      this.MapSize = mapSize;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGameConfigured(this);
    }
  }
}
