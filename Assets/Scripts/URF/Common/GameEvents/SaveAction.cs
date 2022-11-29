namespace URF.Common.GameEvents {
  using System;

  public class SaveAction : EventArgs, IGameEvent {

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleSaveAction(this);
    }
  }
}
