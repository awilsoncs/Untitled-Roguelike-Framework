using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

using URFCommon;

public partial class GameState {

    private readonly List<IRulesSystem> RulesSystems;
    private readonly Dictionary<GameEventType, List<EventHandler>> eventHandlers;

    public void RegisterSystem(IRulesSystem system) {
        // https://stackoverflow.com/questions/3467765/find-methods-that-have-custom-attribute-using-reflection
        RulesSystems.Add(system);
        // Gather up listener methods
        var eventHandlerMethods = system
            .GetType()
            .GetMethods()
            .Where(x => Attribute.GetCustomAttributes(x, typeof (EventHandlerAttribute)).Length > 0);

        foreach(var method in eventHandlerMethods) {
            // may have one or more event types to listen for
            var attributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), false);
            foreach (var attribute in attributes) {
                var eha = (EventHandlerAttribute)attribute;
                eventHandlers[eha.eventType].Add(
                    (IGameState gs, IGameEvent ev) => {method.Invoke(system, new object[]{gs, ev});}
                );
            }
        } 
        entityFactory.UpdateEntitySpec(system.Slots);
    }
}