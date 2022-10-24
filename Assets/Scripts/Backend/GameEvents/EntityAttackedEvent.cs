public class EntityAttackedEvent : IGameEvent {
    /// <summary>The attacking entity's entity ID</summary>
    public int Attacker {get;}
    /// <summary>The defending entity's entity ID</summary>
    public int Defender {get;}
    /// <summary>True if the attack was successful</summary>
    public bool Success {get;}
    public int Damage {get;}

    public GameEventType EventType => GameEventType.EntityAttacked;
    public EntityAttackedEvent (int attacker, int defender, bool success, int damage) {
        Attacker = attacker;
        Defender = defender;
        Success = success;
        Damage = damage;
    }
}