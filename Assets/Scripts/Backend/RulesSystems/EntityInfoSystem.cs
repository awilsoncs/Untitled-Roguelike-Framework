using System.Collections.Generic;

using URFCommon;

/// <summary>
/// Defines basic meta information about entities.
/// </summary>
public class EntityInfoSystem : IRulesSystem
{
    public void GameUpdate(IGameState gameState) {}
    public List<(GameCommandType, CommandHandler)> CommandHandlers => new();
    public List<(GameEventType, EventHandler)> EventHandlers => new();
    public List<(string, SlotType)> Slots => new() {
        ("name", SlotType.String),
    };
}
