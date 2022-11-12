using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using URF.Common.Entities;
using URF.Common.GameEvents;
using URF.Server.GameState;

namespace URF.Server.RulesSystems
{
    public class CombatSystem : BaseRulesSystem
    {
        public override List<Type> Components => new () {
            // todo could create an annotation to register these
            typeof(CombatComponent)
        };

        [EventHandler(GameEventType.AttackCommand)]
        public void HandleAttackCommand(IGameState gs, IGameEvent cm) {
            var ev = (AttackCommand)cm;
            if (!gs.EntityExists(ev.Attacker)) {
                gs.PostError($"Attacking entity {ev.Attacker} does not exist.");
                return;
            }

            if (!gs.EntityExists(ev.Defender)) {
                gs.PostError($"Defender entity {ev.Defender} does not exist.");
                return;
            }

            IEntity attacker = gs.GetEntityById(ev.Attacker);
            CombatComponent attackerCombat = attacker.GetComponent<CombatComponent>();
            IEntity defender = gs.GetEntityById(ev.Defender);
            CombatComponent defenderCombat = defender.GetComponent<CombatComponent>();

            if (!attackerCombat.CanFight) {
                gs.PostError($"{attacker} cannot fight. Check the Entity definition.");
                return;
            }

            if (!defenderCombat.CanFight) {
                gs.PostError($"Illegal attack attempted...(defender {defender})");
                return;
            }

            HandleAttack(gs, attacker, defender);
            if (ev.Attacker == gs.GetMainCharacter().ID) {
                gs.GameUpdate();
            }
        }

        private void HandleAttack(IGameState gs, IEntity attacker, IEntity defender) {
            var attackerCombat = attacker.GetComponent<CombatComponent>();
            var defenderCombat = defender.GetComponent<CombatComponent>();

            var damage = attackerCombat.Damage;
            gs.PostEvent(new EntityAttackedEvent(attacker, defender, true, damage));
            gs.Log($"{attacker} will deal {damage} damage.");
            gs.Log($"{defender} took {damage} damage.");

            var maxHealth = defenderCombat.MaxHealth;
            var currentHealth = defenderCombat.CurrentHealth;

            defenderCombat.CurrentHealth = Math.Min(
                maxHealth, Math.Max(currentHealth - damage, 0));

            if (defenderCombat.CurrentHealth <= 0) {
                gs.Kill(defender);
            }
        }
    }

    public class CombatComponent : BaseComponent
    {
        public bool CanFight {get;set;}
        public int MaxHealth {get;set;}
        public int CurrentHealth {get;set;}
        public int Damage {get;set;}

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