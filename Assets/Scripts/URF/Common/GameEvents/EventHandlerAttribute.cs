using System;

namespace URF.Common.GameEvents {
  [AttributeUsage(AttributeTargets.Method)]
  public class EventHandlerAttribute : Attribute {

    public readonly GameEventType EventType;

    public EventHandlerAttribute(GameEventType evt) {
      EventType = evt;
    }

  }
}
