using System.Collections.Generic;

using URFCommon;

public class DefaultCombatSystem : IRulesSystem
{
    public void GameUpdate(IGameState gameState) {}
    public List<(GameCommandType, CommandHandler)> CommandHandlers => new();
    public List<(GameEventType, EventHandler)> EventHandlers => new();
    public List<(string, SlotType)> Slots => new () {
        ("canFight", SlotType.Boolean),
        ("maxHealth", SlotType.Integer),
        ("currentHealth", SlotType.Integer),
        ( "damage", SlotType.Integer)
    };
}
