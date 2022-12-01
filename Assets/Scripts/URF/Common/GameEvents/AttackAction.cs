namespace URF.Common.GameEvents {
  using System;
  using URF.Common.Entities;

  /// <summary>
  /// Indicate that the player intends to attack another entity.
  /// </summary>
  public class AttackAction : EventArgs, IGameEvent {

    /// <summary>The defender's entity ID</summary>
    public IEntity Attacker {
      get;
    }

    /// <summary>The defender's entity ID</summary>
    public IEntity Defender {
      get;
    }

    public AttackAction(IEntity attacker, IEntity defender) {
      this.Attacker = attacker;
      this.Defender = defender;
    }

    public void Visit(IEventHandler eventHandler) {
      eventHandler.HandleAttackAction(this);
    }
  }
}
