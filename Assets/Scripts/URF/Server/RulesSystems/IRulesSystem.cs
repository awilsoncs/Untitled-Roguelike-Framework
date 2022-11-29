namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.GameEvents;
  using URF.Server.GameState;

  public delegate void EventHandler(IGameState gs, IGameEventArgs ev);

  public delegate void ActionHandler(IGameState gs, IActionEventArgs ev);

  public interface IRulesSystem : IGameEventChannel {

    event EventHandler<IActionEventArgs> GameAction;

    List<Type> Components {
      get;
    }

    void ApplyPlugins(PluginBundle pluginBundle);

  }
}
