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
      }

      this.OnGameEvent(ev.Applied);
    }
  }
}
