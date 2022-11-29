namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.GameEvents;

  public class BaseRulesSystem : IRulesSystem {

    public virtual void ApplyPlugins(PluginBundle pluginBundle) {
      // assume rules systems don't care about plugins
    }

    public virtual List<Type> Components => new();

    public event EventHandler<IGameEventArgs> GameEvent;

    public event EventHandler<IActionEventArgs> GameAction;

    protected virtual void OnGameEvent(IGameEventArgs e) {
      GameEvent?.Invoke(this, e);
    }

    protected virtual void OnGameAction(IActionEventArgs e) {
      GameAction?.Invoke(this, e);
    }

  }
}
