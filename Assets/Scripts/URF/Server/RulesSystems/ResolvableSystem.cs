namespace URF.Server.RulesSystems {
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.Resolvables;

  /// <summary>
  /// Control Item and Ability resolvables.
  /// </summary>
  public class ResolvableSystem : BaseRulesSystem {

    // holding the most recent targeting resolvable while we handle the request.
    private IResolvable targetingResolvable;

    public override void HandleResolvableEvent(ResolvableEvent ev) {
      switch (ev.Step) {
        case ResolvableEvent.ResolvableEventStep.TargetDetermination:
          this.HandleTargetDetermination(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Resolved:
          this.HandleResolved(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Cancelled:
          this.HandleCancelled();
          return;
        default:
          return;
      }
    }

    private void HandleTargetDetermination(IResolvable resolvable) {
      if (resolvable == null) {
        this.OnGameEvent(new GameErrored($"Null passed to ResolvableSystem"));
      }

      if (resolvable.Scope == TargetScope.Self) {
        this.HandleSelfScope(resolvable);
      } else {
        this.HandleCreatureScopes(resolvable);
      }
    }

    private void HandleCreatureScopes(IResolvable resolvable) {
      // we need to calculate legal targets and store the resolvable
      // then, emit a target request
      // when we receive a matching target response, emit a resolved event with the targets
      this.targetingResolvable = resolvable;
      foreach (IEntity entity in resolvable.Agent.VisibleEntities) {
        resolvable.AddLegalTarget(entity);
      }
      this.OnGameEvent(new TargetEvent(resolvable));
    }

    private void HandleSelfScope(IResolvable resolvable) {
      // the target can only be the agent
      resolvable.AddLegalTarget(resolvable.Agent);
      resolvable.ResolveTargets(new HashSet<IEntity>() { resolvable.Agent });
      this.OnGameEvent(
        new ResolvableEvent(resolvable, ResolvableEvent.ResolvableEventStep.Resolved));
    }

    private void HandleResolved(IResolvable resolvable) {
      foreach (IEntity target in resolvable.ResolvedTargets) {
        foreach (IEffect effect in resolvable.Effects) {
          this.OnGameEvent(new EffectEvent(effect, target));
        }
      }
    }

    private void HandleCancelled() {
      this.targetingResolvable = null;
    }

    public override void HandleTargetEvent(TargetEvent targetEvent) {
      if (targetEvent.Method == TargetEvent.TargetEventMethod.Request) {
        return;
      } else if (targetEvent.Method == TargetEvent.TargetEventMethod.Cancelled) {
        this.OnGameEvent(
          new ResolvableEvent(
            this.targetingResolvable,
            ResolvableEvent.ResolvableEventStep.Cancelled));
        return;
      } else if (targetEvent.Resolvable != this.targetingResolvable) {
        this.OnGameEvent(new GameErrored("Mismatched targeting resolvable"));
        this.OnGameEvent(
          new ResolvableEvent(
            this.targetingResolvable,
            ResolvableEvent.ResolvableEventStep.Cancelled));
        return;
      }

      this.targetingResolvable.ResolveTargets(targetEvent.SelectedTargets);

      this.OnGameEvent(
        new ResolvableEvent(
          this.targetingResolvable,
          ResolvableEvent.ResolvableEventStep.Resolved
        )
      );

    }
  }

}
