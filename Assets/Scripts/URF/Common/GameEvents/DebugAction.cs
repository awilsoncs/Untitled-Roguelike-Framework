namespace URF.Common.GameEvents {
  using System;

  public class DebugAction : EventArgs, IGameEvent {

    public enum DebugMethod {

      SpawnCrab

    }

    public DebugMethod Method {
      get;
    }

    private DebugAction(DebugMethod method) {
      this.Method = method;
    }

    public static DebugAction SpawnCrab() {
      return new DebugAction(DebugMethod.SpawnCrab);
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleDebug(this);
    }
  }
}
