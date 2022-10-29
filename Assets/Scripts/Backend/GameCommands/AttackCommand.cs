/// <summary>
/// Indicate that the player intends to attack another entity.
/// </summary>
public struct AttackCommand : IGameCommand {
    /// <summary>The defender's entity ID</summary>
    public int Defender { get; }
    public GameCommandType CommandType => GameCommandType.Attack;

    public AttackCommand (int defender) {
        Defender = defender;
    }
}