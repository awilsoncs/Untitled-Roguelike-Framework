using System;

namespace URFCommon {
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerAttribute : Attribute {
        public GameEventType eventType;

        public EventHandlerAttribute(GameEventType evt) {
            eventType = evt;
        }
    }
}
