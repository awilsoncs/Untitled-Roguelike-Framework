namespace URF.Common.GameEvents {
  using System;

  public class LoadAction : EventArgs, IGameEvent {

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleLoad(this);
    }
  }
}
