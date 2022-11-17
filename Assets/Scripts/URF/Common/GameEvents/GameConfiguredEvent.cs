using System;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameConfiguredEventArgs : EventArgs, IGameEventArgs {

    public Position MapSize { get; }

    public GameEventType EventType => GameEventType.Configure;

    public GameConfiguredEventArgs(Position mapSize) {
      MapSize = mapSize;
    }

  }
}
