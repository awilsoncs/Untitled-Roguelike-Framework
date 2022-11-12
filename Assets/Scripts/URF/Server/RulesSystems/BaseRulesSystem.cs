using System;
using System.Collections.Generic;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class BaseRulesSystem : IRulesSystem {

    public virtual void GameUpdate(IGameState gameState) {}

    public virtual List<Type> Components => new();

  }
}
