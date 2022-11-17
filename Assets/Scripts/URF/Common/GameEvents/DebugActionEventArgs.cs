using System;

namespace URF.Common.GameEvents {
  public class DebugActionEventArgs : EventArgs, IActionEventArgs {

    public enum DebugMethod {

      SpawnCrab

    }

    public GameEventType EventType => GameEventType.DebugCommand;

    public DebugMethod Method { get; }

    private DebugActionEventArgs(DebugMethod method) {
      Method = method;
    }

    public static DebugActionEventArgs SpawnCrab() {
      return new DebugActionEventArgs(DebugMethod.SpawnCrab);
    }

  }
}
