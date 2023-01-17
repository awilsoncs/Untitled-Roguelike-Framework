namespace URF.Effects {
  using System;
  using URF.Common.Entities;

  public class RestoreHealthEffect : BaseEffect {

    private IEntity affected;

    private int magnitude;

    public RestoreHealthEffect(IEntity affected, int magnitude) {
      this.affected = affected;

      if (magnitude < 0) {
        throw new GameEffectException(
          this, "RestoreHealthEffect cannot have magnitude less than 0");
      }
      this.magnitude = magnitude;
    }

    protected override void OnApply() {
      CombatComponent affectedCombat = this.affected.GetComponent<CombatComponent>();
      int maxHealth = affectedCombat.MaxHealth;
      int currentHealth = affectedCombat.CurrentHealth;
      affectedCombat.CurrentHealth = Math.Clamp(currentHealth + this.magnitude, 0, maxHealth);
    }
  }
}
