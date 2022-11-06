using System.Collections.Generic;

public class BaseRulesSystem : IRulesSystem
{
    public virtual void GameUpdate(IGameState gameState) {}
    public virtual List<(string, SlotType)> Slots => new () {};
}
