using System;
using System.Collections.Generic;
using URF.Common;
using URF.Common.GameEvents;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public delegate void EventHandler(IGameState gs, IGameEventArgs ev);

  public delegate void ActionHandler(IGameState gs, IActionEventArgs ev);

  public interface IRulesSystem : IGameEventChannel {

    List<Type> Components { get; }

    void ApplyPlugins(PluginBundle pluginBundle);
    
    void GameUpdate(IGameState gameState);
    
  }
}
