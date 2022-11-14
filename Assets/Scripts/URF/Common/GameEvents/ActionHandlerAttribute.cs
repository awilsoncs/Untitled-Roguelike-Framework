using System;

namespace URF.Common.GameEvents {
  [AttributeUsage(AttributeTargets.Method)]
  public class ActionHandlerAttribute : Attribute {

    // Must be public due to reflection (see usages).
    public readonly GameEventType EventType;

    public ActionHandlerAttribute(GameEventType evt) {
      EventType = evt;
    }

  }
}
