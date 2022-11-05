using System.Collections.ObjectModel;
using System.Collections.Generic;

using URFCommon;

public partial class GameState {
    private readonly List<IRulesSystem> RulesSystems;
    private readonly Dictionary<GameCommandType, List<CommandHandler>> commandHandlers;
    private readonly Dictionary<GameEventType, List<EventHandler>> eventHandlers;

    public void RegisterSystem(IRulesSystem system) {
        RulesSystems.Add(system);
        foreach((GameCommandType ct, CommandHandler ch) in system.CommandHandlers) {
            commandHandlers[ct].Add(ch);
        }
        foreach((GameEventType et, EventHandler eh) in system.EventHandlers) {
            eventHandlers[et].Add(eh);
        }
        entityFactory.UpdateEntitySpec(system.Slots);
    }
}