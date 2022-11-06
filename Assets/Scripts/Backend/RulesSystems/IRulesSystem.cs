using System.Collections.Generic;

using URFCommon;

/// <summary>
/// Callback function for game commands.
/// </summary>
/// <param name="gs">The game state</param>
/// <param name="cm">The initiating command</param>
public delegate void CommandHandler(IGameState gs, IGameCommand cm);
public delegate void EventHandler(IGameState gs, IGameEvent ev);

public interface IRulesSystem {
    void GameUpdate(IGameState gameState);
    List<(string, SlotType)> Slots {get;}
    List<(GameEventType, EventHandler)> EventHandlers {get;}
}