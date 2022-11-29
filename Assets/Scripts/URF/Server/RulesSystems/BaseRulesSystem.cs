namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.GameEvents;
  using URF.Server.GameState;

  public class BaseRulesSystem : BaseGameEventChannel, IRulesSystem {

    public IGameState GameState {
      get;
      set;
    }

    public virtual void ApplyPlugins(PluginBundle pluginBundle) {
      // assume rules systems don't care about plugins
    }

    public virtual List<Type> Components => new();

  }
}
