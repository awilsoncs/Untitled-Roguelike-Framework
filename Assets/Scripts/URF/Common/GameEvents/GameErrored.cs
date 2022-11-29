namespace URF.Common.GameEvents {
  using System;

  /// <summary>
  /// Notify listeners that the game has experienced an error.
  /// </summary>
  public class GameErrored : EventArgs, IGameEvent {
    /// <summary>
    /// The error message
    /// </summary>
    public string Message {
      get;
    }

    public GameErrored(string message) {
      this.Message = message;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleGameErrored(this);
    }
  }
}
