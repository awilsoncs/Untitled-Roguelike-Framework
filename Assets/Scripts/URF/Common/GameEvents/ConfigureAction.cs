namespace URF.Common.GameEvents {

  /// <summary>
  /// Instruct the receiver to configure itself.
  /// </summary>
  public class ConfigureAction : IGameEvent {

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleConfigure(this);
    }
  }
}
