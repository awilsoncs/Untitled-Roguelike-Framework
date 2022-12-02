namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class CombatSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        typeof(CombatComponent)
      };

    public override void HandleAttackAction(AttackAction ev) {
      IEntity attacker = ev.Attacker;
      IEntity defender = ev.Defender;
      CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();

      int damage = attackerCombat.Damage;
      this.OnGameEvent(new EntityAttacked(attacker, defender, true, damage));
      this.OnGameEvent(new TurnSpent(attacker));
    }

    public override void HandleEntityAttacked(EntityAttacked entityAttacked) {
      IEntity defender = entityAttacked.Defender;
      CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();
      int maxHealth = defenderCombat.MaxHealth;
      int currentHealth = defenderCombat.CurrentHealth;
      int damage = entityAttacked.Damage;

      defenderCombat.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));
      if (defenderCombat.CurrentHealth > 0) {
        return;
      }
      this.GameState.Delete(defender);
    }

  }
}
