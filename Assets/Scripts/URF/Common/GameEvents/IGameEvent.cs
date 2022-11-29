namespace URF.Common.GameEvents {
  public interface IGameEvent {

    void Visit(IEventHandler eventHandler) {
      eventHandler.Ignore(this);
    }

  }
}
