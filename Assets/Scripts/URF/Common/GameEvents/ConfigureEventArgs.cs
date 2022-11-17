namespace URF.Common.GameEvents {
  
  /// <summary>
  /// Instruct the receiver to configure itself.
  /// </summary>
  public class ConfigureActionArgs : IActionEventArgs {

    public GameEventType EventType => GameEventType.Configure;

  }
}
