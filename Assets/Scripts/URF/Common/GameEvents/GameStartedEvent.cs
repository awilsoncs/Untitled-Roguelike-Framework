using System;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that a new game has begun.
  /// </summary>
  public class GameStartedEventArgs : EventArgs, IGameEventArgs {

    public Position MapSize { get; }

    public GameEventType EventType => GameEventType.StartGame;

    public GameStartedEventArgs(Position mapSize) {
      MapSize = mapSize;
    }

  }
}
