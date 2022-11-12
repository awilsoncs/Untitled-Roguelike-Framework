using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using URF.Common.GameEvents;
using URF.Server.RulesSystems;
using EventHandler = URF.Server.RulesSystems.EventHandler;

namespace URF.Server.GameState {
  public partial class GameState {

    private readonly List<IRulesSystem> _rulesSystems = new();
    private readonly Dictionary<GameEventType, List<EventHandler>> _eventHandlers = new();

    private void RegisterSystem(IRulesSystem system) {
      // https://stackoverflow.com/questions/3467765/find-methods-that-have-custom-attribute-using-reflection
      _rulesSystems.Add(system);
      // Gather up listener methods
      IEnumerable<MethodInfo> eventHandlerMethods = system.GetType().GetMethods().Where(x =>
        Attribute.GetCustomAttributes(x, typeof(EventHandlerAttribute)).Length > 0);

      foreach(MethodInfo method in eventHandlerMethods) {
        // may have one or more event types to listen for
        object[] attributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), false);
        foreach(object attribute in attributes) {
          EventHandlerAttribute eha = (EventHandlerAttribute)attribute;
          _eventHandlers[eha.eventType].Add((gs, ev) => {
            method.Invoke(system, new object[] { gs, ev });
          });
        }
      }
      _entityFactory.UpdateEntitySpec(system.Components);
    }

  }
}
