using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

using URFCommon;

public partial class GameState {

    private delegate void ListenerInvocation(IGameState gs, IGameCommand cm);
    private readonly List<IRulesSystem> RulesSystems;
    private readonly Dictionary<GameCommandType, List<ListenerInvocation>> commandHandlers;
    private readonly Dictionary<GameEventType, List<EventHandler>> eventHandlers;

    public void RegisterSystem(IRulesSystem system) {
        // https://stackoverflow.com/questions/3467765/find-methods-that-have-custom-attribute-using-reflection
        RulesSystems.Add(system);
        // Gather up listener methods
        Debug.Log($"Found {system.GetType()}");
        Debug.Log($"Found {system.GetType().GetMethods().Count()} methods");
        
        var commandHandlerMethods = system
            .GetType()
            .GetMethods()
            .Where(x => Attribute.GetCustomAttributes(x, typeof (CommandHandlerAttribute)).Length > 0);
        Debug.Log($"Found {commandHandlerMethods.Count()} methods to register");

        foreach(var method in commandHandlerMethods) {
            // may have one or more event types to listen for
            var attributes = method.GetCustomAttributes(typeof(CommandHandlerAttribute), false);
            foreach (var attribute in attributes) {
                var cha = (CommandHandlerAttribute)attribute;
                commandHandlers[cha.commandType].Add(
                    (IGameState gs, IGameCommand cm) => {method.Invoke(system, new object[]{gs, cm});}
                );
                Debug.Log($"Registered method {method}");
            }
        }
        foreach((GameEventType et, EventHandler eh) in system.EventHandlers) {
            eventHandlers[et].Add(eh);
        }
        entityFactory.UpdateEntitySpec(system.Slots);
    }
}