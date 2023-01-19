namespace URF.Server.RulesSystems {
  using URF.Common.Entities;
  using URF.Common.GameEvents;

  public class CombatSystem : BaseRulesSystem {

    public override void HandleAttackAction(AttackAction ev) {
      IEntity attacker = ev.Attacker;
      IEntity defender = ev.Defender;
      int damage = attacker.Damage;
      this.OnGameEvent(new EntityAttacked(attacker, defender, true, damage));
      this.OnGameEvent(new EffectEvent(EffectEvent.EffectType.DamageHealth, damage, defender));
      this.OnGameEvent(new TurnSpent(attacker));
    }

  }
}
