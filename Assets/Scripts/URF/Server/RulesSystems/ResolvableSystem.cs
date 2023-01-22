namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.Resolvables;

  /// <summary>
  /// Control Item and Ability resolvables.
  /// </summary>
  public class ResolvableSystem : BaseRulesSystem {

    public override void HandleResolvableEvent(ResolvableEvent ev) {
      switch (ev.Step) {
        case ResolvableEvent.ResolvableEventStep.TargetDetermination:
          this.HandleTargetDetermination(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Resolved:
          this.HandleResolved(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Cancelled:
          this.HandleCancelled(ev.Resolvable);
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
        // we need to calculate legal targets and store the resolvable
        // then, emit a target request
        // when we receive a matching target response, emit a resolved event with the targets
        this.OnGameEvent(new GameErrored($"Unhandled TargetScope: {resolvable.Scope}"));
      }
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

    private void HandleCancelled(IResolvable resolvable) {
      throw new NotImplementedException();
    }
  }

}
