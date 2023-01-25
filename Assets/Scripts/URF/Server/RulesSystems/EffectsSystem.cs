namespace URF.Server.RulesSystems {
  using System;
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class EffectsSystem : BaseRulesSystem {

    public override void HandleEffectEvent(EffectEvent ev) {
      if (ev.Step == EffectEvent.EffectEventStep.Applied) {
        return;
      }

      IEntity affected = ev.Affected;
      IEffect effect = ev.Effect;
      ICombatStats affectedStats = affected.CombatStats;

      if (effect.Type == EffectType.RestoreHealth) {
        int maxHealth = affectedStats.MaxHealth;
        int currentHealth = affectedStats.CurrentHealth;
        affectedStats.CurrentHealth = Math.Clamp(currentHealth + effect.Magnitude, 0, maxHealth);
        this.OnGameEvent(affected.WasUpdated());
      } else if (effect.Type == EffectType.DamageHealth) {
        int maxHealth = affectedStats.MaxHealth;
        int currentHealth = affectedStats.CurrentHealth;
        int damage = effect.Magnitude;
        affectedStats.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
        if (affectedStats.CurrentHealth <= 0) {
          this.GameState.DeleteEntity(affected);
          return;
        }
        this.OnGameEvent(affected.WasUpdated());
      }

      this.OnGameEvent(ev.Applied);
    }
  }
}
