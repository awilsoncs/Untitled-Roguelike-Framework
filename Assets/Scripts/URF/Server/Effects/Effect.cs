namespace URF.Server.Effects {
  using System;
  using URF.Common.Effects;
  using URF.Common.Entities;

  public class Effect : IEffect {
    public IEntity Agent {
      get;
    }

    public IEntity Source {
      get;
    }

    public IEntity Affected {
      get;
    }

    public IEffectSpec Spec {
      get;
    }

    public Effect(IEntity agent, IEntity source, IEntity affected, IEffectSpec spec) {
      this.Agent = agent ?? throw new ArgumentNullException(nameof(agent));
      this.Source = source ?? throw new ArgumentNullException(nameof(source));
      this.Affected = affected ?? throw new ArgumentNullException(nameof(affected));
      this.Spec = spec ?? throw new ArgumentNullException(nameof(spec));
    }
  }
}
