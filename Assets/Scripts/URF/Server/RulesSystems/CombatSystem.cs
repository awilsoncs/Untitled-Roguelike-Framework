using System;
using System.Collections.Generic;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class CombatSystem : BaseRulesSystem {

    public override List<Type> Components =>
      new() {
        // todo could create an annotation to register these
        typeof(CombatComponent)
      };

    [ActionHandler(GameEventType.AttackCommand)]
    public void HandleAttackAction(IGameState gs, IActionEventArgs cm) {
      AttackActionEventArgs ev = (AttackActionEventArgs)cm;
      IEntity attacker = gs.GetEntityById(ev.Attacker);
      IEntity defender = gs.GetEntityById(ev.Defender);
      HandleAttack(gs, attacker, defender);
      OnGameEvent(new TurnSpentEventArgs(attacker));
    }

    private void HandleAttack(IGameState gs, IEntity attacker, IEntity defender) {
      CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();
      CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();

      int damage = attackerCombat.Damage;
      OnGameEvent(new EntityAttackedEventArgs(attacker, defender, true, damage));

      int maxHealth = defenderCombat.MaxHealth;
      int currentHealth = defenderCombat.CurrentHealth;

      defenderCombat.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));

      if(defenderCombat.CurrentHealth <= 0) { gs.Kill(defender); }
    }

  }

  public class CombatComponent : BaseComponent {

    public bool CanFight { get; set; }

    public int MaxHealth { get; set; }

    public int CurrentHealth { get; set; }

    public int Damage { get; set; }

    public override void Load(GameDataReader reader) {
      CanFight = reader.ReadBool();
      MaxHealth = reader.ReadInt();
      CurrentHealth = reader.ReadInt();
      Damage = reader.ReadInt();
    }

    public override void Save(GameDataWriter writer) {
      writer.Write(CanFight);
      writer.Write(MaxHealth);
      writer.Write(CurrentHealth);
      writer.Write(Damage);
    }

  }
}
