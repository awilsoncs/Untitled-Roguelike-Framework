namespace URF.Common.GameEvents {
    /// <summary>
    /// Indicate that the player intends to attack another entity.
    /// </summary>
    public class AttackCommand : GameEvent {
        /// <summary>The defender's entity ID</summary>
        public int Attacker { get; }
        /// <summary>The defender's entity ID</summary>
        public int Defender { get; }
        public override GameEventType EventType => GameEventType.AttackCommand;
        public override bool IsCommand => true;

        public AttackCommand (int attacker, int defender) {
            Attacker = attacker;
            Defender = defender;
        }
    }
}

