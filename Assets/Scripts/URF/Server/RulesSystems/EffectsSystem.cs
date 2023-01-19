namespace URF.Server.RulesSystems {
  using System;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class EffectsSystem : BaseRulesSystem {

    public override void HandleEffectEvent(EffectEvent ev) {
      if (ev.Step == EffectEvent.EffectEventStep.Applied) {
        return;
      }

      if (ev.Method == EffectEvent.EffectType.RestoreHealth) {
        CombatComponent affectedCombat = ev.Affected.GetComponent<CombatComponent>();
        int maxHealth = affectedCombat.MaxHealth;
        int currentHealth = affectedCombat.CurrentHealth;
        affectedCombat.CurrentHealth = Math.Clamp(currentHealth + ev.Magnitude, 0, maxHealth);
        this.OnGameEvent(ev.Affected.WasUpdated());
      } else if (ev.Method == EffectEvent.EffectType.DamageHealth) {
        IEntity defender = ev.Affected;
        CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();
        int maxHealth = defenderCombat.MaxHealth;
        int currentHealth = defenderCombat.CurrentHealth;
        int damage = ev.Magnitude;

        defenderCombat.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
        if (defenderCombat.CurrentHealth > 0) {
          this.OnGameEvent(defender.WasUpdated());
          return;
        }
        this.GameState.DeleteEntity(defender);
      }

      this.OnGameEvent(ev.Applied);
    }
  }
}
