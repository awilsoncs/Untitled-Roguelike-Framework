using System;
using System.Collections.Generic;

using URFCommon;

public delegate void EventHandler(IGameState gs, IGameEvent ev);

public interface IRulesSystem {
    void GameUpdate(IGameState gameState);
    List<Type> Components {get;}
}