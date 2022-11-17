using System;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Notify listeners that the server has decided on settings.
  /// </summary>
  public class GameStartedEventArgs : EventArgs, IGameEventArgs {

    public GameEventType EventType => GameEventType.Start;

  }
}
