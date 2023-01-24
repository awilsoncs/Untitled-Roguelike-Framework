namespace URF.Server.Resolvables {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;

  public class Resolvable : IResolvable {
    // interface for resolving targets and costs
    // queried for effects once all resolutions are made

    public IEntity Agent {
      get;
    }

    public TargetScope Scope {
      get;
    }

    public IEnumerable<IEffect> Effects {
      get;
    }

    private readonly HashSet<IEntity> legalTargets = new();

    public IEnumerable<IEntity> LegalTargets => this.legalTargets;

    private readonly HashSet<IEntity> resolvedTargets = new();

    public IEnumerable<IEntity> ResolvedTargets => this.resolvedTargets;

    public Resolvable(
      IEntity agent,
      TargetScope scope,
      IEnumerable<IEffect> effects
    ) {
      this.Agent = agent;
      this.Scope = scope;
      this.Effects = effects;
    }

    public void AddLegalTarget(IEntity entity) {
      _ = this.legalTargets.Add(entity);
    }

    public void ResolveTargets(IEnumerable<IEntity> targets) {
      foreach (IEntity target in targets) {
        _ = this.resolvedTargets.Add(target);
      }
    }

    // Get Scope
    // Self
    // One Creature
    // Resolve targets
  }
}
