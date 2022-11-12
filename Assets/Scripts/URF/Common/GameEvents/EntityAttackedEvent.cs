using URF.Common.Entities;

namespace URF.Common.GameEvents {
    public class EntityAttackedEvent : GameEvent {
        /// <summary>The attacking entity's entity ID</summary>
        public IEntity Attacker {get;}
        /// <summary>The defending entity's entity ID</summary>
        public IEntity Defender {get;}
        /// <summary>True if the attack was successful</summary>
        public bool Success {get;}
        public int Damage {get;}

        public override GameEventType EventType => GameEventType.EntityAttacked;
        public EntityAttackedEvent (IEntity attacker, IEntity defender, bool success, int damage) {
            Attacker = attacker;
            Defender = defender;
            Success = success;
            Damage = damage;
        }
    }
}
