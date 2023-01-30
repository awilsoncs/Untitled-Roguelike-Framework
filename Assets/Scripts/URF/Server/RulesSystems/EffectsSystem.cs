namespace URF.Server.RulesSystems {
  using System;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class EffectsSystem : BaseRulesSystem {

    public override void HandleEffectEvent(EffectEvent ev) {

      switch (ev.Step) {
        case EffectEvent.EffectEventStep.Queried:
          this.HandleEffectQueried(ev);
          return;
        case EffectEvent.EffectEventStep.Created:
          this.HandleEffectCreated(ev);
          return;
        case EffectEvent.EffectEventStep.Applied: // fall through
        case EffectEvent.EffectEventStep.Confirmed: // fall through
        case EffectEvent.EffectEventStep.Denied:
          return;
        default:
          this.OnGameEvent(new GameErrored($"Unhandled EffectEvent step: {ev.Step}"));
          return;
      }
    }

    private void HandleEffectQueried(EffectEvent ev) {
      // Here, we determine whether an effect (usually a cost) can be fully applied (paid)
      this.ConfirmPayability(ev.Effect);
    }

    private void ConfirmPayability(IEffect effect) {
      this.OnGameEvent(new EffectEvent(effect, EffectEvent.EffectEventStep.Confirmed));
    }

    private void HandleEffectCreated(EffectEvent ev) {
      IEffect effect = ev.Effect;

      switch (effect.Spec.Type) {
        case EffectType.RestoreHealth:
          this.ApplyRestoreHealth(effect);
          break;
        case EffectType.DamageHealth:
          this.ApplyDamageHealth(effect);
          break;
        case EffectType.ConsumeSource:
          this.ApplyConsumeSource(effect);
          break;
        default:
          this.OnGameEvent(new GameErrored("Unhandled effect type"));
          break;
      }

      this.OnGameEvent(ev.Applied);
    }

    private void ApplyRestoreHealth(IEffect effect) {
      IEntity affected = effect.Affected;
      ICombatStats affectedStats = affected.CombatStats;
      int maxHealth = affectedStats.MaxHealth;
      int currentHealth = affectedStats.CurrentHealth;

      IEffectSpec spec = effect.Spec;
      affectedStats.CurrentHealth = Math.Clamp(currentHealth + spec.Magnitude, 0, maxHealth);
      this.OnGameEvent(affected.WasUpdated());
    }

    private void ApplyDamageHealth(IEffect effect) {
      IEntity affected = effect.Affected;
      ICombatStats affectedStats = affected.CombatStats;
      int maxHealth = affectedStats.MaxHealth;
      int currentHealth = affectedStats.CurrentHealth;

      IEffectSpec spec = effect.Spec;
      int damage = spec.Magnitude;
      affectedStats.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
      if (affectedStats.CurrentHealth > 0) {
        this.OnGameEvent(affected.WasUpdated());
      } else {
        this.GameState.DeleteEntity(affected);
      }
    }

    private void ApplyConsumeSource(IEffect effect) {
      this.OnGameEvent(
        new InventoryEvent(effect.Agent, InventoryEvent.InventoryAction.Consumed, effect.Source));
    }
  }
}
