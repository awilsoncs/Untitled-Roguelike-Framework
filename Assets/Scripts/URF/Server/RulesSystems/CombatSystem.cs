namespace URF.Server.RulesSystems {
  using URF.Common.Effects;
  using URF.Common.Entities;
  using URF.Common.GameEvents;
  using URF.Server.Effects;

  public class CombatSystem : BaseRulesSystem {

    public override void HandleAttackAction(AttackAction ev) {
      IEntity attacker = ev.Attacker;
      IEntity defender = ev.Defender;
      int damage = attacker.CombatStats.Damage;
      this.OnGameEvent(new EntityAttacked(attacker, defender, true, damage));

      Effect damageEffect = new(
        attacker,
        attacker,
        defender,
        EffectType.DamageHealth.WithMagnitude(damage)
      );

      this.OnGameEvent(new EffectEvent(damageEffect));
      this.OnGameEvent(new TurnSpent(attacker));
    }

  }
}
