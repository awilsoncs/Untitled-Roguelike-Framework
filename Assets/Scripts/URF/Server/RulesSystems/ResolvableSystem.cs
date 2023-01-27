namespace URF.Server.RulesSystems {
  using System.Linq;
  using System.Collections.Generic;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.Useables;
  using URF.Server.Resolvables;
  using System;
  using URF.Server.Effects;

  /// <summary>
  /// Handle ResolvableEvents and related TargetEvents and EffectEvents. Emits ResolvableEvents,
  /// TargetEvents, and EffectEvents.
  /// </summary>
  public class ResolvableSystem : BaseRulesSystem {

    // holding the most recent targeting resolvable while we handle the request.
    private IResolvable currentResolvable;
    // holds any costs that still need to be confirmed as payable.
    private readonly List<IEffect> pendingCosts = new();

    /// <summary>
    /// Handle resolvable events.
    /// </summary>
    public override void HandleResolvableEvent(ResolvableEvent ev) {
      if (ev == null) {
        throw new ArgumentNullException("ResolvableEvent cannot be null");
      }

      this.currentResolvable = ev.Resolvable;
      // case order is also the step order
      switch (ev.Step) {
        case ResolvableEvent.ResolvableEventStep.TargetDetermination:
          this.HandleTargetDeterminationEvent(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.CostDetermination:
          this.HandleCostDeterminationEvent(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Resolved:
          this.HandleResolvedEvent(ev.Resolvable);
          return;
        case ResolvableEvent.ResolvableEventStep.Cancelled:
          this.HandleCancelledEvent();
          return;
        default:
          this.OnGameEvent(new GameErrored($"Unhandled resolvable event step"));
          return;
      }
    }

    private void HandleTargetDeterminationEvent(IResolvable resolvable) {
      IUseable useable = resolvable.Useable;

      if (useable.Scope == TargetScope.Self) {
        this.StartSelfScopedTargeting(resolvable);
      } else {
        this.StartCreatureScopedTargeting(resolvable);
      }
    }

    private void StartCreatureScopedTargeting(IResolvable resolvable) {
      // we need to calculate legal targets and store the resolvable
      // then, emit a target request
      // when we receive a matching target response, emit a resolved event with the targets
      this.currentResolvable = resolvable;
      foreach (IEntity entity in resolvable.Agent.VisibleEntities) {
        resolvable.AddLegalTarget(entity);
      }
      this.OnGameEvent(new TargetEvent(resolvable));
    }

    private void StartSelfScopedTargeting(IResolvable resolvable) {
      // the target can only be the agent
      resolvable.AddLegalTarget(resolvable.Agent);
      resolvable.ResolveTarget(resolvable.Agent);
      this.OnGameEvent(
        new ResolvableEvent(resolvable, ResolvableEvent.ResolvableEventStep.CostDetermination));
    }

    private void HandleCostDeterminationEvent(IResolvable resolvable) {
      // It may make sense to eventually move this to its own system
      IEnumerable<IEffectSpec> costs = resolvable.Useable.Costs;

      if (costs.Any()) {
        // The resolvable has costs, so we need to kick off cost payability
        this.BeginCostPayabilityCheck(resolvable);
      } else {
        // Since we don't have any cost, we can go ahead and resolve this ability.
        this.OnGameEvent(
          new ResolvableEvent(resolvable, ResolvableEvent.ResolvableEventStep.Resolved));
      }
    }

    private void BeginCostPayabilityCheck(IResolvable resolvable) {
      IEnumerable<IEffectSpec> costs = resolvable.Useable.Costs;
      foreach (IEffectSpec costSpec in costs) {
        foreach (IEntity target in resolvable.ResolvedTargets) {
          Effect cost = new(resolvable.Agent, resolvable.Source, target, costSpec);
          this.pendingCosts.Add(cost);
          this.OnGameEvent(new EffectEvent(cost, EffectEvent.EffectEventStep.Queried));
        }
      }
    }

    private void HandleResolvedEvent(IResolvable resolvable) {
      IUseable useable = resolvable.Useable;
      foreach (IEffectSpec costSpec in useable.Costs) {
        Effect cost = new(
          resolvable.Agent,
          resolvable.Source,
          resolvable.Source,
          costSpec
        );
        this.OnGameEvent(new EffectEvent(cost));
      }

      foreach (IEntity target in resolvable.ResolvedTargets) {
        foreach (IEffectSpec effectSpec in useable.Effects) {
          Effect effect = new(
            resolvable.Agent,
            resolvable.Source,
            target,
            effectSpec
          );
          this.OnGameEvent(new EffectEvent(effect));
        }
      }
      this.OnGameEvent(new TurnSpent(resolvable.Agent));
    }

    private void HandleCancelledEvent() {
      this.currentResolvable = null;
      this.pendingCosts.Clear();
    }

    public override void HandleTargetEvent(TargetEvent targetEvent) {
      if (targetEvent == null) {
        throw new ArgumentNullException("targetEvent cannot be null");
      } else if (targetEvent.Method == TargetEvent.TargetEventMethod.Request) {
        // We don't care about this one
        return;
      }

      // Cancel conditions
      if (this.IsCancelCondition(targetEvent)) {
        this.OnGameEvent(
          new ResolvableEvent(
            this.currentResolvable,
            ResolvableEvent.ResolvableEventStep.Cancelled));
        return;
      }

      foreach (IEntity target in targetEvent.SelectedTargets) {
        this.currentResolvable.ResolveTarget(target);
      }

      this.OnGameEvent(
        new ResolvableEvent(
          this.currentResolvable,
          ResolvableEvent.ResolvableEventStep.CostDetermination
        )
      );
    }

    // Return whether the given target event represents a cancellation in some way.
    private bool IsCancelCondition(TargetEvent targetEvent) {
      return targetEvent.Method == TargetEvent.TargetEventMethod.Cancelled
       || targetEvent.Resolvable != this.currentResolvable;
    }

    public override void HandleEffectEvent(EffectEvent effectEvent) {
      if (effectEvent == null) {
        throw new ArgumentNullException("effectEvent cannot be null");
      }

      if (
        effectEvent.Step is not EffectEvent.EffectEventStep.Confirmed
        or EffectEvent.EffectEventStep.Denied
      ) {
        // we only care about payability responses here
        return;
      }

      if (effectEvent.Step is EffectEvent.EffectEventStep.Confirmed) {
        this.HandleConfirmedCostEffectEvent(effectEvent);
      } else if (effectEvent.Step is EffectEvent.EffectEventStep.Denied) {
        this.HandleDeniedCostEffectEvent();
      }
    }

    private void HandleConfirmedCostEffectEvent(EffectEvent effectEvent) {
      _ = this.pendingCosts.Remove(effectEvent.Effect);
      if (!this.pendingCosts.Any()) {
        this.ResolveResolvable();
      }
    }

    private void HandleDeniedCostEffectEvent() {
      this.OnGameEvent(
        new ResolvableEvent(
          this.currentResolvable,
          ResolvableEvent.ResolvableEventStep.Cancelled
        )
      );
    }

    private void ResolveResolvable() {
      this.OnGameEvent(
        new ResolvableEvent(
          this.currentResolvable,
          ResolvableEvent.ResolvableEventStep.Resolved
        )
      );
    }

    public override void HandleGameStarted(GameStarted gameStarted) {
      // just in case...
      this.currentResolvable = null;
      this.pendingCosts.Clear();
    }
  }

}
