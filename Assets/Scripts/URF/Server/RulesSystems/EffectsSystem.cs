namespace URF.Server.RulesSystems {
  using System;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class EffectsSystem : BaseRulesSystem {

    public override void HandleEffectEvent(EffectEvent ev) {
      if (ev.Step == EffectEvent.EffectEventStep.Applied) {
        return;
      }

      IEntity affected = ev.Affected;

      if (ev.Method == EffectEvent.EffectType.RestoreHealth) {
        int maxHealth = affected.MaxHealth;
        int currentHealth = affected.CurrentHealth;
        affected.CurrentHealth = Math.Clamp(currentHealth + ev.Magnitude, 0, maxHealth);
        this.OnGameEvent(affected.WasUpdated());
      } else if (ev.Method == EffectEvent.EffectType.DamageHealth) {
        int maxHealth = affected.MaxHealth;
        int currentHealth = affected.CurrentHealth;
        int damage = ev.Magnitude;
        affected.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
        if (affected.CurrentHealth <= 0) {
          this.GameState.DeleteEntity(affected);
          return;
        }
        this.OnGameEvent(affected.WasUpdated());
      }

      this.OnGameEvent(ev.Applied);
    }
  }
}
