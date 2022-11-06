using System;
using System.Collections.Generic;
using URFCommon;

public class CombatSystem : BaseRulesSystem
{
    public override List<(string, SlotType)> Slots => new () {
        ("canFight", SlotType.Boolean),
        ("maxHealth", SlotType.Integer),
        ("currentHealth", SlotType.Integer),
        ( "damage", SlotType.Integer)
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
        IEntity defender = gs.GetEntityById(ev.Defender);

        if (!attacker.GetBoolSlot("canFight")) {
            gs.PostError($"{attacker} cannot fight. Check the Entity definition.");
            return;
        }

        if (!defender.GetBoolSlot("canFight")) {
            gs.PostError($"Illegal attack attempted...(defender {defender})");
            return;
        }

        HandleAttack(gs, attacker, defender);
        if (ev.Attacker == gs.GetMainCharacter().ID) {
            gs.GameUpdate();
        }
    }

    private void HandleAttack(IGameState gs, IEntity attacker, IEntity defender) {
        var damage = attacker.GetIntSlot("damage");
        gs.PostEvent(new EntityAttackedEvent(attacker, defender, true, damage));
        gs.Log($"{attacker} will deal {damage} damage.");
        gs.Log($"{defender} took {damage} damage.");

        var maxHealth = defender.GetIntSlot("maxHealth");
        var currentHealth = defender.GetIntSlot("currentHealth");

        defender.SetSlot(
            "currentHealth",
            Math.Min(maxHealth, Math.Max(currentHealth - damage, 0))
        );

        if (defender.GetIntSlot("currentHealth") <= 0) {
            gs.Kill(defender);
        }
    }
}
