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
      this.Agent = agent ?? throw new ArgumentNullException("Agent cannot be null");
      this.Source = source ?? throw new ArgumentNullException("Source cannot be null");
      this.Affected = affected ?? throw new ArgumentNullException("Affected cannot be null");
      this.Spec = spec ?? throw new ArgumentNullException("Spec cannot be null");
    }
  }
}
