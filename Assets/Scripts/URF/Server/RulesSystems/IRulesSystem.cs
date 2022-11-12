using System;
using System.Collections.Generic;
using URF.Common.GameEvents;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public delegate void EventHandler(IGameState gs, IGameEvent ev);

  public interface IRulesSystem {

    void GameUpdate(IGameState gameState);

    List<Type> Components { get; }

  }
}
