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
      this.OnGameEvent(new EffectEvent(EffectEvent.EffectType.DamageHealth, damage, defender));
      this.OnGameEvent(new TurnSpent(attacker));
    }

  }
}
