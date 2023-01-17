namespace URF.Common {
  using URF.Common.GameEvents;

  /// <summary>
  /// Mixin interface to expose a port for forwarding GameEvents. Intended for use by systems that
  /// want to use objects that also emit events.
  /// </summary>
  public interface IEventForwarder {
    /// <summary>
    /// Forward the event to through this object.
    /// </summary>
    /// <param name="_">Unused parameter</param>
    /// <param name="ev">The event to forward</param>
    void ForwardEvent(object _, IGameEvent ev);
  }
}
