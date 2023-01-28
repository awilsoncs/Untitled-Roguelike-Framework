namespace URF.Server.Resolvables {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.Useables;

  /// <summary>
  /// Default implementation of IResolvable.
  /// </summary>
  public class Resolvable : IResolvable {

    public IEntity Agent {
      get;
    }

    public IEntity Source {
      get;
    }

    public IUseable Useable {
      get;
    }

    private readonly HashSet<IEntity> legalTargets = new();

    public IEnumerable<IEntity> LegalTargets => this.legalTargets;

    private readonly HashSet<IEntity> resolvedTargets = new();

    public IEnumerable<IEntity> ResolvedTargets => this.resolvedTargets;

    public Resolvable(
      IEntity agent,
      IEntity source,
      IUseable useable
    ) {
      this.Agent = agent ?? throw new ArgumentNullException(nameof(agent));
      this.Source = source ?? throw new ArgumentNullException(nameof(source));
      this.Useable = useable ?? throw new ArgumentNullException(nameof(useable));
    }

    public void AddLegalTarget(IEntity entity) {
      if (entity == null) {
        throw new ArgumentNullException(nameof(entity));
      } else if (this.legalTargets.Contains(entity)) {
        throw new ArgumentException($"{nameof(entity)} already a legal target");
      } else {
        // all good!
      }

      if (!this.IsInTargetScope(entity)) {
        throw new ArgumentException($"{nameof(entity)} does not match target scope");
      }

      _ = this.legalTargets.Add(entity);
    }

    private bool IsInTargetScope(IEntity entity) {
      if (this.Useable.Scope is TargetScope.Self) {
        return entity == this.Agent;
      }
      return true;
    }

    public void ResolveTarget(IEntity target) {
      if (target == null) {
        throw new ArgumentNullException(nameof(target));
      } else if (this.resolvedTargets.Contains(target)) {
        throw new ArgumentException($"{nameof(target)} already resolved");
      } else if (!this.legalTargets.Contains(target)) {
        throw new ArgumentException($"{nameof(target)} is not legal");
      } else if (!this.IsTargetResolvable()) {
        throw new ArgumentException($"{nameof(target)} cannot be resolved");
      } else {
        // all good!
      }

      _ = this.resolvedTargets.Add(target);
    }

    private bool IsTargetResolvable() {
      if (this.Useable.Scope is TargetScope.OneCreature) {
        return this.resolvedTargets.Count == 0;
      }
      return true;
    }
  }
}
