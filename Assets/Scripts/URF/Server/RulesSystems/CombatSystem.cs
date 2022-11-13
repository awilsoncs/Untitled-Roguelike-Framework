using System;
using System.Collections.Generic;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Common.Persistence;
using URF.Server.GameState;

namespace URF.Server.RulesSystems {
  public class CombatSystem : BaseRulesSystem {

    public event EventHandler<EntityAttackedEventArgs> EntityAttacked;

    public override List<Type> Components =>
      new() {
        // todo could create an annotation to register these
        typeof(CombatComponent)
      };

    [EventHandler(GameEventType.AttackCommand)]
    public void HandleAttackCommand(IGameState gs, IGameEvent cm) {
      AttackCommand ev = (AttackCommand)cm;
      if(!gs.EntityExists(ev.Attacker)) {
        gs.PostError($"Attacking entity {ev.Attacker} does not exist.");
        return;
      }

      if(!gs.EntityExists(ev.Defender)) {
        gs.PostError($"Defender entity {ev.Defender} does not exist.");
        return;
      }

      IEntity attacker = gs.GetEntityById(ev.Attacker);
      CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();
      IEntity defender = gs.GetEntityById(ev.Defender);
      CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();

      if(!attackerCombat.CanFight) {
        gs.PostError($"{attacker} cannot fight. Check the Entity definition.");
        return;
      }

      if(!defenderCombat.CanFight) {
        gs.PostError($"Illegal attack attempted...(defender {defender})");
        return;
      }

      HandleAttack(gs, attacker, defender);
      if(ev.Attacker == gs.GetMainCharacter().ID) { gs.GameUpdate(); }
    }

    private void HandleAttack(IGameState gs, IEntity attacker, IEntity defender) {
      CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();
      CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();

      int damage = attackerCombat.Damage;
      OnEntityAttacked(attacker, defender, true, damage);

      int maxHealth = defenderCombat.MaxHealth;
      int currentHealth = defenderCombat.CurrentHealth;

      defenderCombat.CurrentHealth = Math.Min(maxHealth, Math.Max(currentHealth - damage, 0));

      if(defenderCombat.CurrentHealth <= 0) { gs.Kill(defender); }
    }

    protected virtual void OnEntityAttacked(
      IEntity attacker,
      IEntity defender,
      bool success,
      int damage
    ) {
      EntityAttackedEventArgs e = new EntityAttackedEventArgs(attacker, defender, success, damage);
      EntityAttacked?.Invoke(this, e);
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
