public class EntityAttackedEvent : IGameEvent {
    /// <summary>The attacking entity's entity ID</summary>
    public int Attacker {get;}
    /// <summary>The defending entity's entity ID</summary>
    public int Defender {get;}

    public GameEventType EventType => GameEventType.EntityAttacked;
    public EntityAttackedEvent (int attacker, int defender) {
        Attacker = attacker;
        Defender = defender;
    }
}