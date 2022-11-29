namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common;
  using URF.Common.GameEvents;
  using URF.Server.GameState;

  public delegate void EventHandler(IGameState gs, IGameEvent ev);


  public interface IRulesSystem : IGameEventChannel, IEventHandler {

    List<Type> Components {
      get;
    }

    IGameState GameState {
      get; set;
    }

    void ApplyPlugins(PluginBundle pluginBundle);
  }
}
