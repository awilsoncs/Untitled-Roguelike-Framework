using System;

namespace URF.Common.GameEvents {
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute {
        public GameEventType eventType;

        public EventHandlerAttribute(GameEventType evt) {
            eventType = evt;
        }
    }
}
