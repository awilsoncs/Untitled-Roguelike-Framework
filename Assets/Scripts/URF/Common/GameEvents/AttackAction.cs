namespace URF.Common.GameEvents {
  using System;

  /// <summary>
  /// Indicate that the player intends to attack another entity.
  /// </summary>
  public class AttackAction : EventArgs, IGameEvent {

    /// <summary>The defender's entity ID</summary>
    public int Attacker {
      get;
    }

    /// <summary>The defender's entity ID</summary>
    public int Defender {
      get;
    }

    public AttackAction(int attacker, int defender) {
      this.Attacker = attacker;
      this.Defender = defender;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleAttackAction(this);
    }
  }
}
