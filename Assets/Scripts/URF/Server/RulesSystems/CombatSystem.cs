namespace URF.Server.RulesSystems {
  using System;
  using System.Collections.Generic;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Common.Persistence;
  using URF.Server.GameState;

  public class CombatSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        // todo could create an annotation to register these
        typeof(CombatComponent)
      };

    public override void HandleAttackAction(AttackAction ev) {
      IEntity attacker = this.GameState.GetEntityById(ev.Attacker);
      IEntity defender = this.GameState.GetEntityById(ev.Defender);
      this.HandleAttack(this.GameState, attacker, defender);
      this.OnGameEvent(new TurnSpent(attacker));
    }

    private void HandleAttack(IGameState gs, IEntity attacker, IEntity defender) {
      CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();
      CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();

      int damage = attackerCombat.Damage;
      this.OnGameEvent(new EntityAttacked(attacker, defender, true, damage));

      int maxHealth = defenderCombat.MaxHealth;
      int currentHealth = defenderCombat.CurrentHealth;

      defenderCombat.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));

      if (defenderCombat.CurrentHealth > 0) {
        return;
      }
      this.OnGameEvent(new EntityKilled(defender));
      gs.Kill(defender);
    }

  }

}
