namespace URF.Common.GameEvents {
  using System;

  public class DebugAction : EventArgs, IGameEvent {

    public enum DebugMethod {

      SpawnCrab,
      Damage

    }

    public DebugMethod Method {
      get;
    }

    public DebugAction(DebugMethod method) {
      this.Method = method;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleDebug(this);
    }
  }
}
