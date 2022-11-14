using System;

namespace URF.Common.GameEvents {
  /// <summary>
  /// Indicate that the player intends to attack another entity.
  /// </summary>
  public class AttackActionEventArgs : EventArgs, IActionEventArgs {

    /// <summary>The defender's entity ID</summary>
    public int Attacker { get; }

    /// <summary>The defender's entity ID</summary>
    public int Defender { get; }

    public GameEventType EventType => GameEventType.AttackCommand;

    public AttackActionEventArgs(int attacker, int defender) {
      Attacker = attacker;
      Defender = defender;
    }

  }
}
